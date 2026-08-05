#pragma warning disable ASPIRECOMPUTE003 // Container registry APIs are preview.

var builder = DistributedApplication.CreateBuilder(args);

var dbPassword = builder.AddParameter("DbPassword", true);
var rustfsAccessKey = builder.AddParameter("rustfs-access-key", secret: true);
var rustfsSecretKey = builder.AddParameter("rustfs-secret-key", secret: true);
var googleClientId = builder.AddParameter("google-client-id", secret: true);
var googleClientSecret = builder.AddParameter("google-client-secret", secret: true);

var registryEndpoint = builder.AddParameter("registry-endpoint");
var registryRepository = builder.AddParameter("registry-repository");
var registry = builder.AddContainerRegistry("registry", registryEndpoint, registryRepository);

var k8s = builder.AddKubernetesEnvironment("k8s")
    .WithContainerRegistry(registry);

var ingress = k8s.AddIngress("public")
    .WithIngressClass("traefik");

const string apiHostname = "api.picator.online";
const string webHostname = "picator.online";
const string externalAuthHostname = "auth.picator.online";
const string clusterIssuerName = "letsencrypt-prod";

var sqlServer = builder.AddPostgres("sql", password: dbPassword)
    // Kubernetes volume names can't contain dots; the auto-generated name derives
    // from the AppHost project name ("Picator.AppHost"), which does. Name it explicitly.
    .WithDataVolume("sql-data")
    .WithLifetime(ContainerLifetime.Persistent);

if (!builder.ExecutionContext.IsPublishMode)
{
    // Pin a stable local port for `aspire run`. Publishing to Kubernetes generates its
    // own Service port for cluster-internal access, so this must not apply there --
    // it previously clobbered the k8s Service's port, breaking picator-api's connection.
    sqlServer.WithHostPort(50925);
}

var postgres = sqlServer.AddDatabase("PicatorDB");

const string rustfsConsoleHostname = "docs.picator.online";
const string rustfsStorageHostname = "kososher.picator.online";

var rustfs = builder.AddRustFs("rustfs", port: 9100, accessKey: rustfsAccessKey, secretKey: rustfsSecretKey)
    .WithDataVolume("rustfs-data")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithExternalHttpEndpoints()
    // Storage (S3-compatible) API also reachable directly at <server-ip>:30101, unencrypted, for
    // external S3 clients that want a raw endpoint. The console endpoint keeps its own
    // auto-assigned NodePort too (unused/harmless) since both endpoints live on the same k8s
    // Service; it's still reached via ingress below.
    .PublishAsKubernetesService(resource =>
    {
        resource.Service!.Spec.Type = "NodePort";
        foreach (var port in resource.Service.Spec.Ports)
        {
            if (port.Name == RustFsResource.PrimaryEndpointName)
            {
                port.NodePort = 30101;
            }
        }
    });

ingress.WithPath(rustfsConsoleHostname, "/", rustfs.GetEndpoint(RustFsResource.ConsoleEndpointName));
// Browser-facing HTTPS path for objects (images, intro frames, avatars) -- the NodePort above
// stays plain HTTP, so anything loaded by a browser on an https page needs this route instead
// or it gets blocked as mixed content.
ingress.WithPath(rustfsStorageHostname, "/", rustfs.GetEndpoint(RustFsResource.PrimaryEndpointName));

var apiService = builder.AddProject<Projects.Picator_Api>("picator-api")
    .WithReference(postgres)
    .WaitFor(postgres)
    .WithExternalHttpEndpoints();

ingress.WithPath(apiHostname, "/", apiService.GetEndpoint("http"));

// gRPC (MagicOnion) — routed via a fixed NodePort instead of the HTTP/HTTPS-only Ingress,
// so the MAUI client can point a plain gRPC channel at <server-ip>:30100.
builder.AddProject<Projects.Picator_Realtime>("picator-realtime")
    .WithReference(postgres)
    .WaitFor(postgres)
    .WithExternalHttpEndpoints()
    .PublishAsKubernetesService(resource =>
    {
        resource.Service!.Spec.Type = "NodePort";
        foreach (var port in resource.Service.Spec.Ports)
        {
            port.NodePort = 30100;
        }
    });

var picatorWeb = builder.AddNextJsApp("picator-web", "../Picator.Web")
    .WithReference(apiService)
    .WaitFor(apiService)
    .WithExternalHttpEndpoints();

// WithDefaultBackend maps to the Ingress spec.defaultBackend field, which this
// cluster's Traefik doesn't seem to honor. A host-less path rule matches all
// hosts the same way, through the more standard rules[] code path instead.
ingress.WithPath("/", picatorWeb.GetEndpoint("http"));

var externalAuth = builder.AddProject<Projects.Picator_ExternalAuth>("picator-externalauth")
    .WithReference(postgres)
    .WaitFor(postgres)
    .WithEnvironment("Google__ClientId", googleClientId)
    .WithEnvironment("Google__ClientSecret", googleClientSecret)
    .WithExternalHttpEndpoints();

ingress.WithPath(externalAuthHostname, "/", externalAuth.GetEndpoint("http"));

// TLS for every public hostname on this shared ingress. Enabling TLS for even one host turns
// on Traefik's HTTPS entrypoint (443) for the whole ingress -- any other host hit over https
// then falls back to Traefik's mismatched default cert and browsers show a security warning.
// So every hostname is covered rather than leaving some on plain HTTP. WithTls captures every
// hostname registered via WithHostname on this ingress (regardless of call order), so a single
// call here produces one multi-SAN certificate covering all four -- not four separate calls,
// which would instead generate four redundant/ambiguous tls[] entries all listing every host.
// Aspire bootstraps a self-signed placeholder for the Secret on first deploy; cert-manager
// (installed separately on the cluster, not by Aspire) replaces it with a real Let's Encrypt
// cert via the ClusterIssuer named below.
ingress.WithIngressAnnotation("cert-manager.io/cluster-issuer", clusterIssuerName);
ingress.WithHostname(externalAuthHostname);
ingress.WithHostname(apiHostname);
ingress.WithHostname(rustfsConsoleHostname);
ingress.WithHostname(rustfsStorageHostname);
ingress.WithHostname(webHostname)
    .WithTls("picator-online-tls");

// builder.AddProject<Projects.Picator_Invitement>("picator-invitement");

builder.Build().Run();

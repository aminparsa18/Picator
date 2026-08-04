#pragma warning disable ASPIRECOMPUTE003 // Container registry APIs are preview.

var builder = DistributedApplication.CreateBuilder(args);

var dbPassword = builder.AddParameter("DbPassword", true);
var rustfsAccessKey = builder.AddParameter("rustfs-access-key", secret: true);
var rustfsSecretKey = builder.AddParameter("rustfs-secret-key", secret: true);

var registryEndpoint = builder.AddParameter("registry-endpoint");
var registryRepository = builder.AddParameter("registry-repository");
var registry = builder.AddContainerRegistry("registry", registryEndpoint, registryRepository);

var k8s = builder.AddKubernetesEnvironment("k8s")
    .WithContainerRegistry(registry);

var ingress = k8s.AddIngress("public")
    .WithIngressClass("traefik");

// TODO: swap "api.localhost" for your real "api.<your-domain>" once DNS is pointed at the server.
const string apiHostname = "api.localhost";

var postgres = builder.AddPostgres("sql", password: dbPassword)
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithHostPort(50925)
    .AddDatabase("PicatorDB");

var rustfs = builder.AddRustFs("rustfs", port: 9100, accessKey: rustfsAccessKey, secretKey: rustfsSecretKey)
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

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

ingress.WithDefaultBackend(picatorWeb.GetEndpoint("http"));

// builder.AddProject<Projects.Picator_ExternalAuth>("picator-externalauth").WithReference(postgres).WaitFor(postgres);

// builder.AddProject<Projects.TempAPITest>("tempapitest").WithReference(apiService).WaitFor(apiService);

// builder.AddProject<Projects.Picator_Invitement>("picator-invitement");

builder.Build().Run();

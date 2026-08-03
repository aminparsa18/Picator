var builder = DistributedApplication.CreateBuilder(args);

var dbPassword = builder.AddParameter("DbPassword", true);
var rustfsAccessKey = builder.AddParameter("rustfs-access-key", secret: true);
var rustfsSecretKey = builder.AddParameter("rustfs-secret-key", secret: true);

var postgres = builder.AddPostgres("sql", password: dbPassword)
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("PicatorDB");

var rustfs = builder.AddRustFs("rustfs", port: 9100, accessKey: rustfsAccessKey, secretKey: rustfsSecretKey)
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var apiService =builder.AddProject<Projects.Picator_Api>("picator-api").WithReference(postgres).WaitFor(postgres);

builder.AddProject<Projects.Picator_Realtime>("picator-realtime").WithReference(postgres).WaitFor(postgres);

// builder.AddProject<Projects.Picator_ExternalAuth>("picator-externalauth").WithReference(postgres).WaitFor(postgres);

// builder.AddProject<Projects.TempAPITest>("tempapitest").WithReference(apiService).WaitFor(apiService);

// builder.AddProject<Projects.Picator_Invitement>("picator-invitement");

builder.Build().Run();

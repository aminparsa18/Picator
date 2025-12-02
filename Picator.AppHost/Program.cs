var builder = DistributedApplication.CreateBuilder(args);

var sqlServerPassword = builder.AddParameter("DbPassword", true);

var sqlServer = builder.AddSqlServer("sql", sqlServerPassword)
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("PicatorDB");

var apiService =builder.AddProject<Projects.Picator_Api>("picator-api").WithReference(sqlServer).WaitFor(sqlServer);

builder.AddProject<Projects.Picator_Realtime>("picator-realtime").WithReference(sqlServer).WaitFor(sqlServer);

builder.AddProject<Projects.Picator_ExternalAuth>("picator-externalauth").WithReference(sqlServer).WaitFor(sqlServer);

builder.AddProject<Projects.TempAPITest>("tempapitest").WithReference(apiService).WaitFor(apiService);

builder.AddProject<Projects.Picator_Invitement>("picator-invitement");

builder.Build().Run();

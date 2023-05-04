using Picator.Common.Extensions;
using Mafiator.IocConfig.Extensions;
using Mafiator.IocConfig.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;

try
{
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(builder.Configuration)
                    .Enrich.FromLogContext()
                    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Hour)
                    .WriteTo.Console(
                        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                    .CreateLogger();

    builder.Host.UseSerilog();

    builder.Services.ConfigureDatabaseConnection(builder.Configuration)
        .ConfigureController()
        .ConfigureSwagger()
        .ConfigureCustomServices(builder.Configuration)
        .ConfigureCustomIdentityServices(builder.Configuration, builder.Environment);

    builder.Services.AddApplicationInsightsTelemetry(options =>
    {
        options.ConnectionString = builder.Configuration["APPINSIGHTS_CONNECTIONSTRING"];
    });


    WebApplication app = builder.Build();

    app.AddCustomMiddleware();
    //app.UseHangfireDashboard("/hangfire", new DashboardOptions
    //{
    //    Authorization = new[]
    //    {
    //    new HangfireAuthorizationFilter()
    //    }
    //});
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, $"Host terminated unexpectedly: {ex.DetailedMessage()}");
}
finally
{
    Log.CloseAndFlush();
}

namespace Mafiator.Api
{
    public partial class Program { }
}
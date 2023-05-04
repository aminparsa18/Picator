using FastEndpoints;
using FastEndpoints.Swagger;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSwag;
using NSwag.Generation.Processors.Security;
using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Data.Dtos.Api;
using Picator.Data;
using Picator.Repository;
using Picator.Repository.Cache;
using Picator.Service.Contracts;
using Picator.Service.Contracts.Avatars;
using Picator.Service.Models;
using Picator.Service.Services;
using Picator.Service.Validations.Users;
using RepoDb;
using Serilog;
using System.Data;
using System.Net;

namespace Picator.Configuration.Extensions;

public static class ServiceCollectionExtentions
{
    public static IServiceCollection ConfigureDatabaseConnection(this IServiceCollection services, IConfiguration configuration)
    {
        Barrel.ApplicationId = "PicatorAPI";

        //  services.AddHangfire(x => x.UseSqlServerStorage(configuration.GetConnectionString("HangfireContext")));
        // services.AddHangfireServer();
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("MafiatorContext")).EnableSensitiveDataLogging());
        GlobalConfiguration.Setup().UseSqlServer();
        services.AddTransient<IDbConnection>(sp => new SqlConnection(configuration.GetConnectionString("MafiatorContext")));
        return services;
    }

    public static IServiceCollection ConfigureController(this IServiceCollection services)
    {
        services.AddAntiforgery();

        services.AddControllers(options =>
        {
            // options.InputFormatters.Insert(0, new MemoryPackInputFormatter());
            // If checkContentType: true then can output multiple format(JSON/MemoryPack, etc...). default is false.
            // options.OutputFormatters.Insert(0, new MemoryPackOutputFormatter(checkContentType: false));
        }).AddJsonOptions(opt => opt.JsonSerializerOptions.PropertyNamingPolicy = null);
        services.AddFastEndpoints();
        services.AddApiVersioning(options =>
        {
            options.ReportApiVersions = true;
            options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
        });
        return services;
    }

    public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
    {
        services.AddSwaggerDoc(document =>
        {
            document.AddSecurity("Bearer", Enumerable.Empty<string>(), new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.ApiKey,
                Name = "Authorization",
                In = OpenApiSecurityApiKeyLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
            });

            document.OperationProcessors.Add(
                new AspNetCoreOperationSecurityScopeProcessor("JWT"));

            document.IgnoreObsoleteProperties = true;

            document.Version = "v1";
            document.PostProcess = process =>
            {
                process.Info.Version = "v1";
                process.Info.Title = "Picator API";
                process.Info.Description = "Legendary online game";
                process.Info.License = new OpenApiLicense
                {
                    Name = "MIT License",
                    Url = "https://opensource.org/licenses/MIT"
                };
                process.Info.Contact = new OpenApiContact
                {
                    Name = "Amin Parsa",
                    Email = "aminparsa18@gmail.com",
                    Url = "https://aminparsa.me"
                };
            };
        }, excludeNonFastEndpoints: true, tagIndex: 0);
        return services;
    }

    public static IServiceCollection ConfigureCustomServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddValidatorsFromAssemblyContaining<UserLoginRequestValidator>();
        services.Scan(scan => scan
        .FromAssemblyOf<IUnitOfWork>()
        .AddClasses(classes => classes.AssignableTo<IUnitOfWork>()).AsMatchingInterface().WithScopedLifetime()
        .FromAssemblyOf<IAvatarService>()
        .AddClasses().AsImplementedInterfaces().WithScopedLifetime());
        services.AddSingleton<INotificationService, NotificationHubService>();
        services.AddDistributedMemoryCache();
        //services.AddStackExchangeRedisCache(options =>
        //{
        //    options.Configuration = configuration.GetSection("Redis").GetValue<string>("Connection");
        //    options.InstanceName = configuration.GetSection("Redis").GetValue<string>("InstanceName");
        //});
        services.AddOptions<NotificationHubOptions>()
            .Configure(configuration.GetSection("NotificationHub").Bind)
            .ValidateDataAnnotations();

        //services.AddSignalR().AddMessagePackProtocol()
        //    .AddAzureSignalR("Endpoint=https://mftor.service.signalr.net;AccessKey=/bXupX8SacE1iztiuK/ZqxdZopEVKtaYTUVb3xUjs9U=;Version=1.0;");

        return services;
    }

    public static void ConfigureCustomIdentityServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
    {
        services.AddIdentityWithOptions(configuration, webHostEnvironment);
        services.AddScoped<ApplicationIdentityErrorDescriber>();
    }

    public static void UseMainMiddlewares(this IApplicationBuilder app)
    {
        app.UseHsts();
        app.UseStatusCodePages(async context =>
        {
            context.HttpContext.Response.ContentType = "application/x-msgpack";
            if (context.HttpContext.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
            {
                await context.HttpContext.Response.WriteAsync(new ApiResult()
                {
                    Errors = new[] { "Token not validated" },
                    StatusCode = ApiResultStatusCode.Unauthorized
                }.ToString());
            }
            else
            {
                await context.HttpContext.Response.WriteAsync(new ApiResult()
                {
                    Errors = new[] { "Internal Error" },
                    StatusCode = ApiResultStatusCode.ServerError
                }.ToString());
            }
        });
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/x-msgpack";
                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature != null)
                {
                    var err = contextFeature.Error;
                    await context.Response.WriteAsync(contextFeature.Error.Message);
                }
            });
        });
        app.UseHttpsRedirection();
        app.UseSerilogRequestLogging();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseFastEndpoints(c =>
        {
            c.Serializer.Options.PropertyNamingPolicy = null;

            c.Endpoints.ShortNames = false;
            c.Endpoints.Filter = ep => ep.EndpointTags?.Contains("exclude") is not true;
            //c.Endpoints.Configurator = (ep) =>
            //{
            //    ep.AddApiExplorerGroupName();
            //};

            c.Versioning.Prefix = "v";
            //c.Versioning.DefaultVersion = 1;

        });
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        app.CallDbInitializer();
        app.UseSwaggerGen();
    }
}
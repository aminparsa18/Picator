using FastEndpoints;
using FastEndpoints.Swagger;
using FluentValidation;
using MemoryPack;
using MemoryPack.AspNetCoreMvcFormatter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Npgsql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
using Serilog;
using System.Data;
using System.Net;
using System.Text.Json;

namespace Picator.Configuration.Extensions;

public static class ServiceCollectionExtentions
{
    public static IServiceCollection ConfigureDatabaseConnection(this WebApplicationBuilder builder, IConfiguration configuration)
    {
        Barrel.ApplicationId = "PicatorAPI";
        builder.AddNpgsqlDbContext<ApplicationDbContext>("PicatorDB", configureDbContextOptions: options =>
        {
            // Picator.Api never registers TickerQ (Picator.Realtime does), so this context's
            // model doesn't know about the ticker.* tables that TickerQ's EF model customizer
            // adds there. EF's pending-model-changes check compares against that fuller model
            // and — seeing the ticker tables "missing" here — refuses to run Migrate() at all.
            // Those tables are already tracked as applied in __EFMigrationsHistory and are
            // exclusively owned/migrated by Realtime, so it's safe to ignore the mismatch here.
            options.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
        });
        builder.Services.AddTransient<IDbConnection>(sp => new NpgsqlConnection(builder.Configuration.GetConnectionString("PicatorContext")));

        //  services.AddHangfire(x => x.UseSqlServerStorage(configuration.GetConnectionString("HangfireContext")));
        // services.AddHangfireServer();
        //services.AddDbContext<ApplicationDbContext>(options =>
        //    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")).EnableSensitiveDataLogging());
        return builder.Services;
    }

    public static IServiceCollection ConfigureController(this IServiceCollection services)
    {
        services.AddAntiforgery();

        services.AddControllers(options =>
        {
            options.InputFormatters.Insert(0, new MemoryPackInputFormatter());
            // If checkContentType: true then can output multiple format(JSON/MemoryPack, etc...). default is false.
            options.OutputFormatters.Insert(0, new MemoryPackOutputFormatter(checkContentType: false));
        });
        // EmptyRequest (used by EndpointWithoutRequest<T>) fails MemoryPackRequestBinder<T>'s
        // `new()` constraint, so it needs the plain default binder instead of the MemoryPack one.
        services.AddSingleton<IRequestBinder<EmptyRequest>, RequestBinder<EmptyRequest>>();
        services.AddSingleton(typeof(IRequestBinder<>), typeof(MemoryPackRequestBinder<>)).AddFastEndpoints();
        return services;
    }

    public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
    {
        services.SwaggerDocument(document =>
        {
            document.DocumentSettings = s =>
            {
                s.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
                });
                s.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
                s.Version = "v1";
                s.PostProcess = process =>
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
            };
        });
        return services;
    }

    public static IServiceCollection ConfigureCustomServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
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
            context.HttpContext.Response.ContentType = "application/x-memorypack";
            if (context.HttpContext.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
            {
                await context.HttpContext.Response.WriteAsync(new ApiResult()
                {
                    Errors = ["Token not validated"],
                    StatusCode = ApiResultStatusCode.Unauthorized
                }.ToString());
            }
            else
            {
                await context.HttpContext.Response.WriteAsync(new ApiResult()
                {
                    Errors = ["Internal Error"],
                    StatusCode = ApiResultStatusCode.ServerError
                }.ToString());
            }
        });
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/x-memorypack";
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
            c.Serializer.ResponseSerializer = (rsp, dto, cType, jsc, ct) =>
            {
                var isSwaggerRequest = rsp.HttpContext.Items["IsSwaggerRequest"];
                if ((isSwaggerRequest is bool ss && !ss) || isSwaggerRequest == null)
                {
                    rsp.ContentType = "application/x-memorypack";
                    return MemoryPackSerializer.SerializeAsync(dto.GetType(), rsp.Body, dto, cancellationToken: ct).AsTask();
                }
                rsp.ContentType = "application/json";
                return JsonSerializer.SerializeAsync(rsp.Body, dto, cancellationToken: ct);
            };

            c.Serializer.RequestDeserializer = async (req, tDto, jsc, ct) =>
            {
                // Hybrid: accept JSON *or* MemoryPack
                try
                {
                    return await MemoryPackSerializer.DeserializeAsync(tDto, req.Body, cancellationToken: ct);
                }
                catch (MemoryPackSerializationException)
                {
                    // rewind so other components (if any) aren’t broken
                    req.Body.Position = 0;
                    return await JsonSerializer.DeserializeAsync(req.Body, tDto);
                }

                //if (first == '{' || first == '[')
                //{
                //    // JSON (Swagger UI, Postman, etc.)
                //    return await JsonSerializer.DeserializeAsync(ms, tDto, new JsonSerializerOptions
                //    {
                //        PropertyNameCaseInsensitive = true
                //    }, ct);

                // Binary MemoryPack (your own clients)

            };

            // c.Serializer.Options.PropertyNamingPolicy = null;

            c.Endpoints.ShortNames = false;
            c.Endpoints.Filter = ep => ep.EndpointTags?.Contains("exclude") is not true;
            //c.Endpoints.Configurator = (ep) =>
            //{
            //    ep.AddApiExplorerGroupName();
            //};

            c.Versioning.Prefix = "v";
            //c.Versioning.DefaultVersion = 1;

        }).UseOpenApi().UseSwaggerGen();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        app.CallDbInitializer();
    }
}
using FluentValidation;
using Npgsql;
using Picator.Configuration.Extensions;
using Picator.Data;
using Picator.Repository;
using Picator.Service.Contracts.Games;
using Picator.Service.Contracts.Matchmaking;
using Picator.Realtime.Services;
using Picator.Service.Services.Games;
using Picator.Service.Services.Matchmaking;
using Picator.Service.Validations.Users;
using Serilog;
using System.Data;
using TickerQ.DependencyInjection;
using TickerQ.EntityFrameworkCore.Customizer;
using TickerQ.EntityFrameworkCore.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(builder.Configuration)
                    .Enrich.FromLogContext()
                    .WriteTo.Console(
                        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                    .CreateLogger();

builder.Host.UseSerilog();
builder.AddNpgsqlDbContext<ApplicationDbContext>("PicatorDB");
builder.Services.AddTransient<IDbConnection>(sp => new NpgsqlConnection(builder.Configuration.GetConnectionString("PicatorContext")));
builder.Services.AddScoped<IGameCreateService, GameCreateService>();
builder.Services.AddScoped<IMatchmakingService, MatchmakingService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddValidatorsFromAssemblyContaining<UserLoginRequestValidator>();

builder.Services.AddJwtBearerAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

builder.Services.AddGrpc();
builder.Services.AddMagicOnion();
builder.Services.AddSingleton<MatchmakingGroupService>();

builder.Services.AddTickerQ(options =>
{
    options.AddOperationalStore(efOptions =>
    {
        efOptions.UseApplicationDbContext<ApplicationDbContext>(ConfigurationType.UseModelCustomizer);
    });
});

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
app.UseSerilogRequestLogging();

app.UseAuthentication();
app.UseAuthorization();

app.UseTickerQ();

app.MapMagicOnionService();
app.MapGet("/", () => "Ready to rock realtimev2!");

app.Run();
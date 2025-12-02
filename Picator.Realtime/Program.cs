using FluentValidation;
using Microsoft.Data.SqlClient;
using Picator.Data;
using Picator.Repository;
using Picator.Service.Contracts.Games;
using Picator.Service.Services.Games;
using Picator.Service.Validations.Users;
using Serilog;
using System.Data;

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
builder.AddSqlServerDbContext<ApplicationDbContext>("PicatorDB");
builder.Services.AddTransient<IDbConnection>(sp => new SqlConnection(builder.Configuration.GetConnectionString("PicatorContext")));
builder.Services.AddScoped<IGameCreateService, GameCreateService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddValidatorsFromAssemblyContaining<UserLoginRequestValidator>();

builder.Services.AddGrpc();
builder.Services.AddMagicOnion();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
app.UseSerilogRequestLogging();

app.MapMagicOnionService();
app.MapGet("/", () => "Ready to rock realtimev2!");

app.Run();
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//Log.Logger = new LoggerConfiguration()
//                    .ReadFrom.Configuration(builder.Configuration)
//                    .Enrich.FromLogContext()
//                    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Hour)
//                    .WriteTo.Console(
//                        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
//                    .CreateLogger();

//builder.Host.UseSerilog();
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//      options.UseSqlServer(builder.Configuration.GetConnectionString("PicatorContext")).EnableSensitiveDataLogging());
//GlobalConfiguration.Setup().UseSqlServer();
//builder.Services.AddTransient<IDbConnection>(sp => new SqlConnection(builder.Configuration.GetConnectionString("PicatorContext")));
//builder.Services.AddValidatorsFromAssemblyContaining<UserLoginRequestValidator>();
//builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
//builder.Services.AddScoped<IGameCreateService, GameCreateService>();

builder.Services.AddGrpc();
builder.Services.AddMagicOnion();

var app = builder.Build();

// Configure the HTTP request pipeline.
//app.UseSerilogRequestLogging();

app.MapMagicOnionService();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
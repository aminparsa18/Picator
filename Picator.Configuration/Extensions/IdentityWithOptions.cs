using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Picator.Data;
using Picator.Entities.Identity;
using Picator.Service.Contracts.Identity;
using Picator.Service.Services.Identity;
using System.Text;

namespace Picator.Configuration.Extensions;

public static class AddIdentityWithOptionsExtensions
{
    public static IServiceCollection AddIdentityWithOptions(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
    {
        services.AddIdentity<User, Role>(
            options =>
            {
                //Configure Password
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._+";
                options.User.RequireUniqueEmail = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(20);
                options.Lockout.MaxFailedAccessAttempts = 3;
            })
         .AddEntityFrameworkStores<ApplicationDbContext>()
         .AddErrorDescriber<ApplicationIdentityErrorDescriber>()
         .AddDefaultTokenProviders();
        services.AddTransient<ITokenService, TokenService>();
        // configure strongly typed settings objects
        var appSettingsSection = configuration.GetSection("Jwt");
        services.Configure<Jwt>(appSettingsSection);
        // configure jwt authentication
        var appSettings = appSettingsSection.Get<Jwt?>();
        var key = Encoding.ASCII.GetBytes(appSettings?.Secret);
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            RequireExpirationTime = false,
            ValidateLifetime = true
        };

        services.AddSingleton(tokenValidationParameters);
        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddGoogle(g =>
        {
            g.ClientId = "562105913185-p9mp4k328ved7hpmnd8pevruei7chejd.apps.googleusercontent.com";
            g.ClientSecret = "_Tb1heniOnV_pOKBtFcXyPX5";
            g.CallbackPath = "/home/AuthRedirect";
            g.SaveTokens = true;
        }).AddFacebook(f =>
        {
            f.AppId = "2790942157883682";
            f.AppSecret = "49e5c75d1ab421c43df3fd0e0db8e307";
            f.SaveTokens = true;
        }).AddJwtBearer(x =>
        {
            x.SaveToken = true;
            x.TokenValidationParameters = tokenValidationParameters;
            x.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        context.Response.Headers.Add("Token-Expired", "true");
                    return Task.CompletedTask;
                },
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];

                    // If the request is for our hub...
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) &&
                    path.StartsWithSegments("/gamehub"))
                    {
                        // Read the token out of the query string
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                }
            };
        });
        var keysFolder = Path.Combine(webHostEnvironment.ContentRootPath, "Keys");
        services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo(keysFolder))
            .SetApplicationName("Mafiator");
        return services;
    }
}
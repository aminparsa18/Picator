using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Picator.Data;
using System.Text;

namespace Picator.Configuration.Extensions;

public static class JwtBearerAuthenticationExtensions
{
    /// <summary>
    /// Registers JWT bearer authentication using the shared "Jwt" config section (symmetric HMAC-SHA256, issuer/audience unchecked).
    /// Shared between projects that need to validate tokens issued by <see cref="Picator.Service.Services.Identity.TokenService"/>
    /// without pulling in the full ASP.NET Identity stack.
    /// </summary>
    public static IServiceCollection AddJwtBearerAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var appSettingsSection = configuration.GetSection("Jwt");
        services.Configure<Jwt>(appSettingsSection);
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
                }
            };
        });

        return services;
    }
}

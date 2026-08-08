using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Microsoft.IdentityModel.Tokens;
using Picator.Data;
using Picator.Entities.Identity;
using Picator.ExternalAuth.Services;
using Picator.ExternalAuth.Services.Token;
using Picator.Repository;
using System.Data;
using System.Net;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

builder.AddNpgsqlDbContext<ApplicationDbContext>("PicatorDB");
builder.Services.AddTransient<IDbConnection>(sp => new NpgsqlConnection(builder.Configuration.GetConnectionString("PicatorContext")));

builder.Services.AddIdentity<User, Role>(
           options =>
           {
               //Configure Password
               options.Password.RequireDigit = true;
               options.Password.RequiredLength = 6;
               options.Password.RequiredUniqueChars = 1;
               options.Password.RequireLowercase = false;
               options.Password.RequireNonAlphanumeric = false;
               options.Password.RequireUppercase = false;
               options.User.RequireUniqueEmail = true;
               options.SignIn.RequireConfirmedEmail = true;
               options.SignIn.RequireConfirmedPhoneNumber = false;

               options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(20);
               options.Lockout.MaxFailedAccessAttempts = 3;
           })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddErrorDescriber<ApplicationIdentityErrorDescriber>()
        .AddDefaultTokenProviders();

builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IExternalLoginService, ExternalLoginService>();

// configure strongly typed settings objects
var appSettingsSection = builder.Configuration.GetSection("Jwt");
builder.Services.Configure<Jwt>(appSettingsSection);

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
builder.Services.AddSingleton(tokenValidationParameters);

builder.Services.AddAuthentication(o =>
{
    o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
    .AddCookie(c =>
    {
        c.LoginPath = "/mobileauth/google-login";
    })
    .AddGoogle(GoogleDefaults.AuthenticationScheme, g =>
    {
        g.ClientId = builder.Configuration["Google:ClientId"]
            ?? throw new InvalidOperationException("Missing configuration: Google:ClientId");
        g.ClientSecret = builder.Configuration["Google:ClientSecret"]
            ?? throw new InvalidOperationException("Missing configuration: Google:ClientSecret");
        g.SaveTokens = true;
    });
builder.Services.AddAuthorization();

var app = builder.Build();

app.MapDefaultEndpoints();
app.UseRouting();

app.UseCookiePolicy(new CookiePolicyOptions()
{
    MinimumSameSitePolicy = SameSiteMode.Lax
});
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/mobileauth/{scheme}", async ([FromRoute] string scheme, [FromServices] IExternalLoginService _userLoginService, HttpContext ctx) =>
{
    var auth = await ctx.AuthenticateAsync(scheme);
    if (!auth.Succeeded || auth?.Principal == null || !auth.Principal.Identities.Any(id => id.IsAuthenticated))
    {
        // Not authenticated, challenge
        await ctx.ChallengeAsync(scheme);
    }
    else
    {
        var claims = auth.Principal.Identities.FirstOrDefault()?.Claims;
        var email = claims?.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;
        var name = claims?.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Name)?.Value;
        var authResult = await _userLoginService.Login(email, name);
        // Get parameters to send back to the callback
        var qs = new Dictionary<string, string>
                {
                    { "name", name },
                    { "email", email },
                    { "token", authResult.Token},
                    { "refresh_token", authResult.RefreshToken},
                    {"has_name", authResult.HasName.ToString() }
                };
        // Build the result url
        var url = "app://auth.pctor?" + string.Join(
            "&",
            qs.Where(kvp => !string.IsNullOrEmpty(kvp.Value) && kvp.Value != "-1")
            .Select(kvp => $"{WebUtility.UrlEncode(kvp.Key)}={WebUtility.UrlEncode(kvp.Value)}"));

        // Redirect to final url
        ctx.Response.Redirect(url);
    }
});
app.Run();

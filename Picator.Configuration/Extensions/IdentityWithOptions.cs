using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Picator.Data;
using Picator.Entities.Identity;
using Picator.Service.Contracts.Identity;
using Picator.Service.Services.Identity;

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
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
                options.SignIn.RequireConfirmedPhoneNumber = false;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(20);
                options.Lockout.MaxFailedAccessAttempts = 3;
            })
         .AddEntityFrameworkStores<ApplicationDbContext>()
         .AddErrorDescriber<ApplicationIdentityErrorDescriber>()
         .AddDefaultTokenProviders();
        services.AddTransient<ITokenService, TokenService>();
        services.AddJwtBearerAuthentication(configuration);

        var keysFolder = Path.Combine(webHostEnvironment.ContentRootPath, "Keys");
        services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo("/tmp/keys"))
            .SetApplicationName("Picator");

        return services;
    }
}
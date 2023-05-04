using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Picator.Service.Contracts.Identity;

namespace Picator.Configuration.Extensions;

public static class AppBuilderExtensions
{
    public static void CallDbInitializer(this IApplicationBuilder app)
    {
        var scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
        using var scope = scopeFactory.CreateScope();
        var identityDbInitialize = scope.ServiceProvider.GetService<IIdentityDbInitializer>();
        identityDbInitialize?.Initialize();
        identityDbInitialize?.SeedData();
    }
}
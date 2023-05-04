using Microsoft.AspNetCore.Builder;
using Picator.Configuration.Extensions;

namespace Picator.Configuration.Middleware;

public static class AddMiddlewareExtentions
{
    public static void AddCustomMiddleware(this IApplicationBuilder app)
    {
        app.UseMainMiddlewares();
        //var rewriteOptions = new RewriteOptions();
        //rewriteOptions.Rules.Add(new NonWwwRewriteRule());
        //app.UseRewriter(rewriteOptions);

    }
}
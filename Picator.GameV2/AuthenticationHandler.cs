using Picator.Game.Cache;
using System.Net.Http.Headers;

namespace Picator.GameV2;

/// <summary>
/// Automatically adds the Bearer token to every request.
/// This is cleaner than manually managing the Authorization header.
/// </summary>
public class AuthenticationHandler : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Automatically add the current token to each request
        if (Barrel.Current.Exists("Token"))
        {
            var token = Barrel.Current.Get<string>("Token");
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        return base.SendAsync(request, cancellationToken);
    }
}
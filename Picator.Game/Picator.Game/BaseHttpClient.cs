using Picator.Game.Cache;
using Picator.Game.Extensions;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Picator.Game;

public class BaseHttpClient
{
    private static HttpClient _instance;
    private static readonly object _padlock = new();

    private BaseHttpClient()
    {
    }

    public static HttpClient Instance
    {
        get
        {
            lock (_padlock)
            {
                return _instance ??= CreateInstance();
            }
        }
    }

    private static HttpClient CreateInstance()
    {
        var client = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
        if (Barrel.Current.Exists("Token"))
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", Barrel.Current.Get<string>("Token"));
        }
        client.AddDefaultMessagePackAcceptHeader();
        return client;
    }
}

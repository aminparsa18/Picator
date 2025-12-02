using Picator.Game.Extensions;
using Picator.GameV2;

namespace Picator.Game;

/// <summary>
/// Provides a singleton HttpClient instance with automatic authentication handling.
/// Uses DelegatingHandler to automatically add tokens to each request.
/// </summary>
public class BaseHttpClient
{
    private static HttpClient _instance;
    private static readonly SemaphoreSlim _semaphore = new(1, 1);

    private BaseHttpClient()
    {
    }

    public static HttpClient Instance
    {
        get
        {
            // Fast path - no locking if already created
            if (_instance != null)
                return _instance;

            // Slow path - need to create instance
            _semaphore.Wait();
            try
            {
                // Double-check after acquiring semaphore
                return _instance ??= CreateInstance();
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }

    /// <summary>
    /// Resets the HttpClient instance asynchronously.
    /// </summary>
    public static async Task ResetAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            _instance?.Dispose();
            _instance = null;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Resets the HttpClient instance synchronously.
    /// </summary>
    public static void Reset()
    {
        _semaphore.Wait();
        try
        {
            _instance?.Dispose();
            _instance = null;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private static HttpClient CreateInstance()
    {
        var authHandler = new AuthenticationHandler
        {
            InnerHandler = new HttpClientHandler()
        };

        var client = new HttpClient(authHandler)
        {
            Timeout = TimeSpan.FromSeconds(15)
        };

        client.AddDefaultMessagePackAcceptHeader();
        return client;
    }
}
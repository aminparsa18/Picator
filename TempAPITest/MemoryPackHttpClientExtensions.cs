using MemoryPack;
using Polly;
using Polly.Retry;
using System.Net;
using System.Net.Http.Headers;

namespace Picator.Game.Extensions;

public static class MemoryPackHttpClientExtensions
{
    public static readonly string ContentTypeString = "application/x-memorypack";
    private static readonly MediaTypeWithQualityHeaderValue _contentTypeMediaTypeHeaderValue = new(ContentTypeString);
    private static AsyncRetryPolicy<HttpResponseMessage> _refreshTokenPolicy;

    public static Action NavigateToLogin { get; set; }

    public static void AddDefaultMessagePackAcceptHeader(this HttpClient client)
    {
        ArgumentNullException.ThrowIfNull(client);
        if (!client.DefaultRequestHeaders.Accept.Contains(_contentTypeMediaTypeHeaderValue))
            client.DefaultRequestHeaders.Accept.Add(_contentTypeMediaTypeHeaderValue);
    }

    public static async Task<T> GetFromMemoryPackAsync<T>(this HttpClient client, string requestUri, bool isPublicEndpoint = false)
    {
        ArgumentNullException.ThrowIfNull(client);

        if (!isPublicEndpoint)
            await EnsureValidTokenAsync();

        //  CreateRefreshTokenPolicy();
        //var response = await _refreshTokenPolicy.ExecuteAsync(async context =>
        //{
            using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            request.Headers.Add("Accept", ContentTypeString);
            // No need to manually add Authorization header - AuthenticationHandler does it!
            var response = await client.SendAsync(request);
        //}, CancellationToken.None).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsMemoryPackAsync<T>();
    }

    public static async Task<HttpResponseMessage> PostAsMemoryPackAsync<T>(this HttpClient client, string requestUri, T value, bool isPublicEndpoint = false)
    {
        ArgumentNullException.ThrowIfNull(client);

        if (!isPublicEndpoint)
            await EnsureValidTokenAsync();

        using var content = new ByteArrayContent(MemoryPackSerializer.Serialize(value));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/x-memorypack");

        HttpResponseMessage response;

        if (isPublicEndpoint)
        {
            response = await client.PostAsync(requestUri, content).ConfigureAwait(false);
        }
        else
        {
          //  CreateRefreshTokenPolicy();
            response = await _refreshTokenPolicy.ExecuteAsync(async context =>
            {
                using var contentCopy = new ByteArrayContent(MemoryPackSerializer.Serialize(value));
                contentCopy.Headers.ContentType = new MediaTypeHeaderValue("application/x-memorypack");
                return await client.PostAsync(requestUri, contentCopy, context);
            }, CancellationToken.None).ConfigureAwait(false);
        }

       // response.EnsureSuccessStatusCode();
        return response;
    }

    public static async Task<HttpResponseMessage> PutAsMemoryPackAsync<T>(this HttpClient client, Uri requestUri, T value, bool isPublicEndpoint = false)
    {
        ArgumentNullException.ThrowIfNull(client);

        if (!isPublicEndpoint)
            await EnsureValidTokenAsync();

        using var content = new ByteArrayContent(MemoryPackSerializer.Serialize(value));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/x-memorypack");
        return await client.PutAsync(requestUri, content).ConfigureAwait(false);
    }

    private static async Task EnsureValidTokenAsync()
    {
        //if (!Barrel.Current.Exists("Token"))
        //{
        //    NavigateToLogin?.Invoke();
        //    throw new UnauthorizedAccessException("No authentication token found. Please log in.");
        //}

        //var token = Barrel.Current.Get<string>("Token");
        //if (string.IsNullOrEmpty(token))
        //{
        //    NavigateToLogin?.Invoke();
        //    throw new UnauthorizedAccessException("Authentication token is invalid. Please log in.");
        //}

        //if (IsTokenExpiringSoon())
        //{
        //    var refreshResult = await RefreshTokenAsync();
        //    if (!refreshResult.IsSuccess)
        //    {
        //        NavigateToLogin?.Invoke();
        //        throw new UnauthorizedAccessException("Failed to refresh authentication token. Please log in.");
        //    }
        //}
    }

    private static bool IsTokenExpiringSoon()
    {
        //if (Barrel.Current.Exists("TokenExpiration"))
        //{
        //    var expirationTime = Barrel.Current.Get<DateTime>("TokenExpiration");
        //    return DateTime.UtcNow.AddMinutes(1) >= expirationTime;
        //}
        return false;
    }


    
}
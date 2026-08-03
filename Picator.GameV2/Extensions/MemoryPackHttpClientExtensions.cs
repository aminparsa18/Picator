using CommunityToolkit.Maui.Alerts;
using MemoryPack;
using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Api.Auth;
using Picator.Common.Data.Dtos.Data.Dtos.Api;
using Picator.Game.Cache;
using Picator.Game.Constants;
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

    public static async Task<T> GetFromMemoryPackAsync<T>(this HttpClient client, Uri requestUri, bool isPublicEndpoint = false)
    {
        ArgumentNullException.ThrowIfNull(client);

        if (!isPublicEndpoint)
            await EnsureValidTokenAsync();

        CreateRefreshTokenPolicy();
        var response = await _refreshTokenPolicy.ExecuteAsync(async context =>
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            request.Headers.Add("Accept", ContentTypeString);
            // No need to manually add Authorization header - AuthenticationHandler does it!
            return await client.SendAsync(request, context);
        }, CancellationToken.None).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsMemoryPackAsync<T>();
    }

    public static async Task<HttpResponseMessage> PostAsMemoryPackAsync<T>(this HttpClient client, Uri requestUri, T value, bool isPublicEndpoint = false)
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
            CreateRefreshTokenPolicy();
            response = await _refreshTokenPolicy.ExecuteAsync(async context =>
            {
                using var contentCopy = new ByteArrayContent(MemoryPackSerializer.Serialize(value));
                contentCopy.Headers.ContentType = new MediaTypeHeaderValue("application/x-memorypack");
                return await client.PostAsync(requestUri, contentCopy, context);
            }, CancellationToken.None).ConfigureAwait(false);
        }

        response.EnsureSuccessStatusCode();
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
        if (!Barrel.Current.Exists("Token"))
        {
            NavigateToLogin?.Invoke();
            throw new UnauthorizedAccessException("No authentication token found. Please log in.");
        }

        var token = Barrel.Current.Get<string>("Token");
        if (string.IsNullOrEmpty(token))
        {
            NavigateToLogin?.Invoke();
            throw new UnauthorizedAccessException("Authentication token is invalid. Please log in.");
        }

        if (IsTokenExpiringSoon())
        {
            var refreshResult = await RefreshTokenAsync();
            if (!refreshResult.IsSuccess)
            {
                NavigateToLogin?.Invoke();
                throw new UnauthorizedAccessException("Failed to refresh authentication token. Please log in.");
            }
        }
    }

    private static bool IsTokenExpiringSoon()
    {
        if (Barrel.Current.Exists("TokenExpiration"))
        {
            var expirationTime = Barrel.Current.Get<DateTime>("TokenExpiration");
            return DateTime.UtcNow.AddMinutes(1) >= expirationTime;
        }
        return false;
    }

    private static void CreateRefreshTokenPolicy()
    {
        _refreshTokenPolicy ??= Policy
            .HandleResult<HttpResponseMessage>(message => message.StatusCode == HttpStatusCode.Unauthorized)
            .RetryAsync(1, async (result, retryCount, context) =>
            {
                var refreshTokenResult = await RefreshTokenAsync();
                if (!refreshTokenResult.IsSuccess)
                {
                    NavigateToLogin?.Invoke();
                    throw new UnauthorizedAccessException(string.Join(", ", refreshTokenResult.Errors));
                }
            });
    }

    private static async Task<ApiResult> RefreshTokenAsync()
    {
        if (!Barrel.Current.Exists("Token") || !Barrel.Current.Exists("RefreshToken"))
        {
            return new ApiResult()
            {
                IsSuccess = false,
                Errors = new[] { "Token or RefreshToken does not exist" },
                StatusCode = ApiResultStatusCode.NotFound
            };
        }

        var refreshTokenRequest = new RefreshTokenRequest()
        {
            Token = Barrel.Current.Get<string>("Token"),
            RefreshToken = Barrel.Current.Get<string>("RefreshToken")
        };

        try
        {
            var response = await RefreshToken(refreshTokenRequest);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsMemoryPackAsync<AuthResult>();
                if (!data.IsSuccess)
                    return new ApiResult() { IsSuccess = false, Errors = data.Errors };

                Barrel.Current.Add("Token", data.Token, TimeSpan.FromDays(7));
                Barrel.Current.Add("RefreshToken", data.RefreshToken, TimeSpan.FromDays(150));
                Barrel.Current.Add("TokenExpiration", DateTime.UtcNow.AddDays(7), TimeSpan.FromDays(7));

                // No need to manually update HttpClient header - AuthenticationHandler will get the new token automatically!

                return new ApiResult() { IsSuccess = true };
            }
            else
            {
                var data = await response.Content.ReadAsMemoryPackAsync<ApiResult>();
                return new ApiResult() { IsSuccess = false, Errors = data.Errors };
            }
        }
        catch (Exception ex)
        {
            return new ApiResult()
            {
                IsSuccess = false,
                Errors = new[] { $"Error refreshing token: {ex.Message}" }
            };
        }
    }

    private static Task<HttpResponseMessage> RefreshToken(RefreshTokenRequest refreshTokenRequest)
    {
        return BaseHttpClient.Instance.PostAsMemoryPackAsync(
            new Uri(UrlConstants.ApiUrl + "users/refresh"), refreshTokenRequest, isPublicEndpoint: true);
    }
}
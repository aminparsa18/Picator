using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Api.Auth;
using Picator.Common.Data.Dtos.Users;
using Picator.Game;
using Picator.Game.Constants;
using Picator.Game.Extensions;
using System.Net.Http;

namespace Mafiator.Common.Client.Services.Users;

/// <inheritdoc/>
public class UsersApiService : IUsersApiService
{
    /// <inheritdoc/>
    public Task<HttpResponseMessage> ConfirmEmail(ConfirmEmailRequest request)
    {
        return BaseHttpClient.Instance.PostAsMemoryPackAsync(
            new Uri($"{UrlConstants.BaseUrl}users/confirm"), request);
    }

    /// <inheritdoc/>
    public Task<HttpResponseMessage> Login(UserLoginRequest userLogin)
    {
        return BaseHttpClient.Instance.PostAsMemoryPackAsync(new Uri($"{UrlConstants.BaseUrl}users/login"),
            userLogin);
    }

    /// <inheritdoc/>
    public Task<HttpResponseMessage> RefreshToken(RefreshTokenRequest refreshTokenRequest)
    {
        return BaseHttpClient.Instance.PostAsMemoryPackAsync(new Uri($"{UrlConstants.BaseUrl}users/refresh"),
            refreshTokenRequest);
    }

    /// <inheritdoc/>
    public Task<HttpResponseMessage> RegisterUser(RegisterUserRequest registerUserDto)
    {
        return BaseHttpClient.Instance.PostAsMemoryPackAsync(new Uri($"{UrlConstants.BaseUrl}users/register"),
            registerUserDto);
    }

    /// <inheritdoc/>
    public Task<HttpResponseMessage> UpdateProfile(UpdateProfileRequest profile)
    {
        return BaseHttpClient.Instance.PostAsMemoryPackAsync(
            new Uri($"{UrlConstants.BaseUrl}users/update"), profile);
    }

    /// <inheritdoc/>
    public Task<ApiResult<UserDetailsResult>> GetUser()
    {
        return BaseHttpClient.Instance.GetFromMemoryPackAsync<ApiResult<UserDetailsResult>>(
            new Uri($"{UrlConstants.BaseUrl}users"));
    }

    /// <inheritdoc/>
    public Task<ApiResult<UserStatusResult>> GetUserStatus()
    {
        return BaseHttpClient.Instance.GetFromMemoryPackAsync<ApiResult<UserStatusResult>>(
            new Uri($"{UrlConstants.BaseUrl}users/status"));
    }

    /// <inheritdoc/>
    public Task<ApiResult<ValidateUserResult>> ValidateUser(string userCode)
    {
        return BaseHttpClient.Instance.GetFromMemoryPackAsync<ApiResult<ValidateUserResult>>(
            new Uri($"{UrlConstants.BaseUrl}api/User/ValidateUser?username={userCode}"));
    }
}
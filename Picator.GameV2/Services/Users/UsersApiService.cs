using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Api.Auth;
using Picator.Common.Data.Dtos.Users;
using Picator.Game;
using Picator.Game.Constants;
using Picator.Game.Extensions;

namespace Mafiator.Common.Client.Services.Users;

/// <inheritdoc/>
public class UsersApiService : IUsersApiService
{
    /// <inheritdoc/>
    public Task<HttpResponseMessage> ConfirmEmail(ConfirmEmailRequest request)
    {
        return BaseHttpClient.Instance.PostAsMemoryPackAsync(
            new Uri($"{UrlConstants.ApiUrl}users/confirm"), request, true);
    }

    /// <inheritdoc/>
    public Task<HttpResponseMessage> Login(UserLoginRequest userLogin)
    {
        return BaseHttpClient.Instance.PostAsMemoryPackAsync(new Uri($"{UrlConstants.ApiUrl}users/login"),
            userLogin, true);
    }

    /// <inheritdoc/>
    public Task<HttpResponseMessage> RefreshToken(RefreshTokenRequest refreshTokenRequest)
    {
        return BaseHttpClient.Instance.PostAsMemoryPackAsync(new Uri($"{UrlConstants.ApiUrl}users/refresh"),
            refreshTokenRequest, true);
    }

    /// <inheritdoc/>
    public Task<HttpResponseMessage> RegisterUser(RegisterUserRequest registerUserDto)
    {
        return BaseHttpClient.Instance.PostAsMemoryPackAsync(new Uri($"{UrlConstants.ApiUrl}users/register"),
            registerUserDto, true);
    }

    /// <inheritdoc/>
    public Task<HttpResponseMessage> UpdateProfile(UpdateProfileRequest profile)
    {
        return BaseHttpClient.Instance.PostAsMemoryPackAsync(
            new Uri($"{UrlConstants.ApiUrl}users/update"), profile);
    }

    /// <inheritdoc/>
    public Task<HttpResponseMessage> ChangePassword(ChangePasswordRequest request)
    {
        // TODO: backend endpoint not implemented yet — this route does not exist on the API.
        return BaseHttpClient.Instance.PostAsMemoryPackAsync(
            new Uri($"{UrlConstants.ApiUrl}users/change-password"), request);
    }

    /// <inheritdoc/>
    public Task<ApiResult<UserDetailsResult>> GetUser()
    {
        return BaseHttpClient.Instance.GetFromMemoryPackAsync<ApiResult<UserDetailsResult>>(
            new Uri($"{UrlConstants.ApiUrl}users"));
    }

    /// <inheritdoc/>
    public Task<ApiResult<UserStatusResult>> GetUserStatus()
    {
        return BaseHttpClient.Instance.GetFromMemoryPackAsync<ApiResult<UserStatusResult>>(
            new Uri($"{UrlConstants.ApiUrl}users/status"));
    }

    /// <inheritdoc/>
    public Task<ApiResult<ValidateUserResult>> ValidateUser(string userCode)
    {
        return BaseHttpClient.Instance.GetFromMemoryPackAsync<ApiResult<ValidateUserResult>>(
            new Uri($"{UrlConstants.ApiUrl}api/User/ValidateUser?username={userCode}"));
    }
}
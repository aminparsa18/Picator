using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Api.Auth;
using Picator.Common.Data.Dtos.Users;
using System.Net.Http;

namespace Mafiator.Common.Client.Services.Users;

/// <summary>
/// API service provides methods to retrieve/handle users.
/// </summary>
public interface IUsersApiService
{
    /// <summary>
    /// Confirms phone number.
    /// </summary>
    /// <param name="request">Confirm phone request.</param>
    /// <returns>HTTP response message.</returns>
    Task<HttpResponseMessage> ConfirmEmail(ConfirmEmailRequest request);

    /// <summary>
    /// Logs in user.
    /// </summary>
    /// <param name="request">User login request.</param>
    /// <returns>HTTP response message.</returns>
    Task<HttpResponseMessage> Login(UserLoginRequest request);

    /// <summary>
    /// Refreshes expired jwt token.
    /// </summary>
    /// <param name="refreshTokenRequest">Refresh token request.</param>
    /// <returns>HTTP response message.</returns>
    Task<HttpResponseMessage> RefreshToken(RefreshTokenRequest refreshTokenRequest);

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="request">Register user request.</param>
    /// <returns>HTTP response message.</returns>
    Task<HttpResponseMessage> RegisterUser(RegisterUserRequest request);

    /// <summary>
    /// Updates profile details.
    /// </summary>
    /// <param name="request">Update profile request.</param>
    /// <returns>HTTP response message.</returns>
    Task<HttpResponseMessage> UpdateProfile(UpdateProfileRequest request);

    /// <summary>
    /// Get user details.
    /// </summary>
    /// <returns>User details api result.</returns>
    Task<ApiResult<UserDetailsResult>> GetUser();

    /// <summary>
    /// Get user game status.
    /// </summary>
    /// <returns>User game status api result.</returns>
    Task<ApiResult<UserStatusResult>> GetUserStatus();

    /// <summary>
    /// Validates if user exist or not by code.
    /// </summary>
    /// <param name="userCode">User code.</param>
    /// <returns>User details api result.</returns>
    Task<ApiResult<ValidateUserResult>> ValidateUser(string userCode);
}
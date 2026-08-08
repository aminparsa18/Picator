using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Api.Auth;

namespace Picator.Service.Contracts.RefreshTokens;

public interface IRefreshTokenService
{
    Task<AuthResult> Refresh(RefreshTokenRequest request);

    /// <summary>
    /// Invalidates the given refresh token so it can no longer be used to mint new access tokens.
    /// </summary>
    /// <param name="userId">Authenticated user's identifier.</param>
    /// <param name="request">Logout request holding the refresh token to invalidate.</param>
    Task<ApiResult> Logout(string userId, LogoutRequest request);
}
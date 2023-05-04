using Picator.Common.Data.Dtos.Api.Auth;

namespace Picator.Service.Contracts.RefreshTokens;

public interface IRefreshTokenService
{
    Task<AuthResult> Refresh(RefreshTokenRequest request);
}
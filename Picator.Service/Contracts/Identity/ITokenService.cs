using Picator.Common.Server.Auth;
using Picator.Entities.Identity;
using System.Security.Claims;

namespace Picator.Service.Contracts.Identity;

public interface ITokenService
{
    GenerateTokenResult GenerateAccessToken(User user,List<Claim> claims);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}
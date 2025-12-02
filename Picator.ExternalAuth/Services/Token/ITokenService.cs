using Picator.Entities.Identity;
using Picator.ExternalAuth.Models;
using System.Security.Claims;

namespace Picator.ExternalAuth.Services.Token;

public interface ITokenService
{
    GenerateTokenResult GenerateAccessToken(User user, List<Claim> claims);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}
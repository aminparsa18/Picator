using Microsoft.AspNetCore.Identity;
using Picator.Common.Helpers;
using Picator.Entities.Identity;
using Picator.Entities.Models;
using Picator.ExternalAuth.Services.Token;
using Picator.Repository;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Picator.ExternalAuth.Services;

public class ExternalLoginService : IExternalLoginService
{
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<User> _userManager;

    public ExternalLoginService(ITokenService tokenService, UserManager<User> userManager, IUnitOfWork unitOfWork)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _unitOfWork = unitOfWork;
    }

    public async Task<ExtrenalAuthResult> Login(string email, string? name)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            // create new user
            user = new User
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                Id = Guid.NewGuid(),
                Code = RandomHelper.CreateRandomText(10),
                Score = 100,
                DisplayName = string.IsNullOrWhiteSpace(name) ? null : name.Trim()
            };
            var createdUser = await _userManager.CreateAsync(user, RandomHelper.CreateRandomText(10));
            if (!createdUser.Succeeded)
            {
                Console.WriteLine("error creating external user : " + createdUser.Errors.Select(x => x.Description));
                return new ExtrenalAuthResult()
                {
                };
            }
            await _userManager.AddToRoleAsync(user, Constants.PlayerRole);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Role, Constants.PlayerRole),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Name, user.Id.ToString())
        };
        var tokenResult = _tokenService.GenerateAccessToken(user, claims);
        var refreshToken = new RefreshToken()
        {
            JwtId = tokenResult.JwtId,
            UserId = user.Id,
            ExpirationDate = DateTime.UtcNow.AddMonths(6),
            Token = _tokenService.GenerateRefreshToken()
        };
        await _unitOfWork.RefreshToken.Add(refreshToken);
        return new ExtrenalAuthResult()
        {
            IsSuccess = true,
            Token = tokenResult.Token,
            RefreshToken = refreshToken.Token,
            HasName = !string.IsNullOrEmpty(user.DisplayName)
        };
    }
}
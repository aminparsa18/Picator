using FluentValidation;
using Picator.Common.Data.Dtos.Api.Auth;
using Picator.Common.Data.Dtos.Data.Dtos.Api;
using Picator.Data;
using Picator.Entities.Identity;
using Picator.Entities.Models;
using Picator.Repository;
using Picator.Service.Contracts.Identity;
using Picator.Service.Contracts.RefreshTokens;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Picator.Service.Services.RefreshTokens;
public class RefreshTokenService : IRefreshTokenService
{
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<RefreshTokenRequest> _validator;
    private readonly UserManager<User> _userManager;


    public RefreshTokenService(ITokenService tokenService, IUnitOfWork unitOfWork, IValidator<RefreshTokenRequest> validator, UserManager<User> userManager)
    {
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _userManager = userManager;
    }

    public async Task<AuthResult> Refresh(RefreshTokenRequest request)
    {
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return new AuthResult
            {
                StatusCode = ApiResultStatusCode.BadRequest,
                Errors = validationResult.Errors.Select(e => e.ErrorMessage)
            };
        //pass refresh token
        var principle = _tokenService.GetPrincipalFromExpiredToken(request.Token);
        if (principle == null)
        {
            return new AuthResult()
            {
                StatusCode = ApiResultStatusCode.BadRequest,
                Errors = new[] { "Invalid Token" }
            };
        }

        var storedRefreshTokens = await _unitOfWork.RefreshToken.GetByToken(request.RefreshToken);
        if (!storedRefreshTokens.Any())
        {
            return new AuthResult()
            {
                StatusCode = ApiResultStatusCode.NotFound,
                Errors = new[] { "Refresh Token does not exist" }
            };
        }

        var storedRefreshToken = storedRefreshTokens.FirstOrDefault();
        if (DateTime.UtcNow > storedRefreshToken.ExpirationDate)
        {
            return new AuthResult()
            {
                StatusCode = ApiResultStatusCode.LogicError,
                Errors = new[] { "Refresh Token has expired" }
            };
        }

        if (storedRefreshToken.IsInvalidated)
        {
            return new AuthResult()
            {
                StatusCode = ApiResultStatusCode.BadRequest,
                Errors = new[] { "Refresh Token Invalidated" }
            };
        }

        //Tode: must be enabled after issue investigation.
        //if (storedRefreshToken.IsUsed)
        //{
        //    return new AuthResult()
        //    {
        //        StatusCode = ApiResultStatusCode.BadRequest,
        //        Errors = new[] { "This refresh token has been used" }
        //    };
        //}

        var jti = principle.Claims.Single(c => c.Type == JwtRegisteredClaimNames.Jti).Value;
        if (storedRefreshToken.JwtId != jti)
        {
            return new AuthResult()
            {
                StatusCode = ApiResultStatusCode.BadRequest,
                Errors = new[] { "This refresh token does not match this JWT" }
            };
        }

        await _unitOfWork.RefreshToken.SetUsed(storedRefreshToken.Id.ToString());
        var user = await _userManager.FindByIdAsync(principle.FindFirstValue(ClaimTypes.Name));
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
        await _unitOfWork.RefreshToken.AddFast(refreshToken);
        return new AuthResult()
        {
            IsSuccess = true,
            Token = tokenResult.Token,
            RefreshToken = refreshToken.Token
        };
    }
}
using FluentValidation;
using Picator.Common.Data.Dtos.Api.Auth;
using Picator.Common.Data.Dtos.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Users;
using Picator.Data;
using Picator.Entities.Identity;
using Picator.Entities.Models;
using Picator.Repository;
using Picator.Service.Contracts.Identity;
using Picator.Service.Contracts.Users;
using Microsoft.AspNetCore.Identity;
using RepoDb;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Picator.Service.Services.Users;

public class UserLoginService : IUserLoginService
{
    private readonly IDbConnection _dbConnection;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UserLoginRequest> _validator;
    private readonly UserManager<User> _userManager;

    public UserLoginService(IDbConnection dbConnection, ITokenService tokenService, IValidator<UserLoginRequest> validator,
          UserManager<User> userManager, IUnitOfWork unitOfWork)
    {
        _dbConnection = dbConnection;
        _validator = validator;
        _tokenService = tokenService;
        _userManager = userManager;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthResult> Login(UserLoginRequest request)
    {
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return new AuthResult
            {
                StatusCode = ApiResultStatusCode.BadRequest,
                Errors = validationResult.Errors.Select(e => e.ErrorMessage)
            };
        var users = await _dbConnection.ExecuteQueryAsync<User>(
            "SELECT TOP 1 * FROM [Users] WHERE Username = @username", new { username = request.Username });
        if (!users.Any())
        {
            return new AuthResult()
            {
                StatusCode = ApiResultStatusCode.Unauthorized,
                Errors = new[] { "Login data is not correct." }
            };
        }

        var user = users.FirstOrDefault();
        var userHasValidPassword = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!userHasValidPassword)
        {
            return new AuthResult()
            {
                StatusCode = ApiResultStatusCode.Unauthorized,
                Errors = new[] { "Login data is not correct." }
            };
        }

        if (!user.PhoneNumberConfirmed)
        {
            var token = await _userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);
            //smsSender.SendAuthSmsAsync(token, user.PhoneNumber);
            return new AuthResult()
            {
                StatusCode = ApiResultStatusCode.Forbidden,
                Errors = new[] { "Phone number is not confirmed" },
                Token = user.PhoneNumber
            };
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
        await _unitOfWork.RefreshToken.AddFast(refreshToken);
        return new AuthResult()
        {
            IsSuccess = true,
            Token = tokenResult.Token,
            RefreshToken = refreshToken.Token
        };
    }
}
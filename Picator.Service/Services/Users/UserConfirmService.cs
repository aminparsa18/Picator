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
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Picator.Service.Services.Users;

public class UserConfirmService : IUserConfirmService
{
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<ConfirmPhoneRequest> _validator;
    private readonly RoleManager<Role> _roleManager;
    private readonly UserManager<User> _userManager;

    public UserConfirmService(IValidator<ConfirmPhoneRequest> validator, RoleManager<Role> roleManager,
        IUnitOfWork unitOfWork, ITokenService tokenService, UserManager<User> userManager)
    {
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task<AuthResult> Confirm(ConfirmPhoneRequest request)
    {
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return new AuthResult
            {
                StatusCode = ApiResultStatusCode.BadRequest,
                Errors = validationResult.Errors.Select(e => e.ErrorMessage)
            };
        var existingUser = await _userManager.Users.FirstOrDefaultAsync(e => e.PhoneNumber == request.PhoneNo);
        if (existingUser == null)
        {
            return new AuthResult()
            {
                StatusCode = ApiResultStatusCode.Unauthorized,
                Errors = new[] { "User does not exist" }
            };
        }

        var result = await _userManager.ChangePhoneNumberAsync(existingUser, request.PhoneNo, request.Token);
        if (!result.Succeeded)
            return new AuthResult()
            {
                StatusCode = ApiResultStatusCode.BadRequest,
                Errors = result.Errors.Select(s => s.Description)
            };

        var claims = new List<Claim>
        {
            new(ClaimTypes.Role, Constants.PlayerRole),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Name, existingUser.Id.ToString())
        };

        var userClaims = await _userManager.GetClaimsAsync(existingUser);
        claims.AddRange(userClaims);
        var userRoles = await _userManager.GetRolesAsync(existingUser);
        foreach (var userRole in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, userRole));
            var role = await _roleManager.FindByNameAsync(userRole);
            if (role == null) continue;
            var roleClaims = await _roleManager.GetClaimsAsync(role);
            foreach (var roleClaim in roleClaims)
            {
                if (claims.Contains(roleClaim))
                    continue;
                claims.Add(roleClaim);
            }
        }

        var tokenResult = _tokenService.GenerateAccessToken(existingUser, claims);
        var refreshToken = new RefreshToken()
        {
            JwtId = tokenResult.JwtId,
            UserId = existingUser.Id,
            ExpirationDate = DateTime.UtcNow.AddMonths(6),
            Token = _tokenService.GenerateRefreshToken()
        };
        await _unitOfWork.RefreshToken.AddFast(refreshToken);
        existingUser.PhoneNumberConfirmed = true;
        await _userManager.UpdateAsync(existingUser);
        return new AuthResult()
        {
            IsSuccess = true,
            Token = tokenResult.Token,
            RefreshToken = refreshToken.Token
        };
    }
}
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Users;
using Picator.Common.Helpers;
using Picator.Data;
using Picator.Data.Mappers;
using Picator.Entities.Identity;
using Picator.Service.Contracts.Users;
using RepoDb;
using System.Data;

namespace Picator.Service.Services.Users;

public class UserRegisterService : IUserRegisterService
{
    private readonly IDbConnection _dbConnection;
    private readonly IValidator<RegisterUserRequest> _validator;
    private readonly UserManager<User> _userManager;

    public UserRegisterService(IDbConnection dbConnection, IValidator<RegisterUserRequest> validator, UserManager<User> userManager)
    {
        _dbConnection = dbConnection;
        _validator = validator;
        _userManager = userManager;
    }

    public async Task<ApiResult> Register(RegisterUserRequest request)
    {
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return new ApiResult
            {
                StatusCode = ApiResultStatusCode.BadRequest,
                Errors = validationResult.Errors.Select(e => e.ErrorMessage)
            };
        var users = await _dbConnection.ExecuteQueryAsync<User>(
           "SELECT TOP 1 * FROM [Users] WHERE Username = @username", new { username = request.UserName });
        if (users.Any())
        {
            return new ApiResult()
            {
                StatusCode = ApiResultStatusCode.Conflict,
                Errors = new[] { "User already exist." }
            };
        }

        var user = new UserMapper().RegisterRequestToUser(request);
        user.Email = user.UserName;
        user.Id = Guid.NewGuid(); ;
        user.Code = RandomHelper.CreateRandomText(10);
        user.Score = 100;
        var createdUser = await _userManager.CreateAsync(user, request.Password ?? "");
        if (!createdUser.Succeeded)
        {
            return new ApiResult()
            {
                StatusCode = ApiResultStatusCode.LogicError,
                Errors = createdUser.Errors.Select(x => x.Description)
            };
        }

        await _userManager.AddToRoleAsync(user, Constants.PlayerRole);
        var token = await _userManager.GenerateChangeEmailTokenAsync(user, user.Email ?? "");
        return new ApiResult()
        {
            IsSuccess = true
        };
    }
}
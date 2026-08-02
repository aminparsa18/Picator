using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Users;
using Picator.Entities.Identity;
using Picator.Service.Contracts.Users;

namespace Picator.Service.Services.Users;

public class UserChangePasswordService : IUserChangePasswordService
{
    private readonly IValidator<ChangePasswordRequest> _validator;
    private readonly UserManager<User> _userManager;

    public UserChangePasswordService(IValidator<ChangePasswordRequest> validator, UserManager<User> userManager)
    {
        _validator = validator;
        _userManager = userManager;
    }

    public async Task<ApiResult> ChangePassword(string userId, ChangePasswordRequest request)
    {
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return new ApiResult
            {
                StatusCode = ApiResultStatusCode.BadRequest,
                Errors = validationResult.Errors.Select(e => e.ErrorMessage)
            };

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new ApiResult()
            {
                StatusCode = ApiResultStatusCode.Unauthorized,
                Errors = new[] { "User does not exist" }
            };
        }

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword ?? "", request.NewPassword ?? "");
        if (!result.Succeeded)
        {
            return new ApiResult()
            {
                StatusCode = ApiResultStatusCode.LogicError,
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        return new ApiResult() { IsSuccess = true };
    }
}

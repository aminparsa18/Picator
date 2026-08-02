using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Users;
using Picator.Entities.Identity;
using Picator.Service.Contracts.Users;

namespace Picator.Service.Services.Users;

public class UserUpdateDisplayNameService : IUserUpdateDisplayNameService
{
    private readonly IValidator<UpdateDisplayNameRequest> _validator;
    private readonly UserManager<User> _userManager;

    public UserUpdateDisplayNameService(IValidator<UpdateDisplayNameRequest> validator, UserManager<User> userManager)
    {
        _validator = validator;
        _userManager = userManager;
    }

    public async Task<ApiResult> UpdateDisplayName(string userId, UpdateDisplayNameRequest request)
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

        user.DisplayName = request.Name;
        await _userManager.UpdateAsync(user);
        return new ApiResult() { IsSuccess = true };
    }
}

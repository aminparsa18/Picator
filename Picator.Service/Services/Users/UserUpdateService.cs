using FluentValidation;
using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Users;
using Picator.Entities.Identity;
using Picator.Service.Contracts.Users;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;

namespace Picator.Service.Services.Users;

public class UserUpdateService : IUserUpdateService
{
    private readonly IValidator<UpdateProfileRequest> _validator;
    private readonly UserManager<User> _userManager;

    public UserUpdateService(IValidator<UpdateProfileRequest> validator, UserManager<User> userManager)
    {
        _validator = validator;
        _userManager = userManager;
    }

    public async Task<ApiResult> Update(string userId, UpdateProfileRequest request)
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
        user.Avatar = request.Image;
        await _userManager.UpdateAsync(user);
        return new ApiResult() { IsSuccess = true };
    }
}
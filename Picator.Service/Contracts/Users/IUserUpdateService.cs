using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Users;

namespace Picator.Service.Contracts.Users;

public interface IUserUpdateService
{
    Task<ApiResult> Update(string userId, UpdateProfileRequest request);
}
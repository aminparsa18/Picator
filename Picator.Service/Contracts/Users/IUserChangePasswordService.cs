using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Users;

namespace Picator.Service.Contracts.Users;

public interface IUserChangePasswordService
{
    Task<ApiResult> ChangePassword(string userId, ChangePasswordRequest request);
}

using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Users;

namespace Picator.Service.Contracts.Users;

public interface IUserService
{
    Task<ApiResult<UserDetailsResult>> GetDetails(string userId);

    Task<ApiResult<UserStatusResult>> GetStatus(string userId);
}
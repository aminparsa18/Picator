using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Users;

namespace Picator.Service.Contracts.Users;

public interface IUserUpdateDisplayNameService
{
    Task<ApiResult> UpdateDisplayName(string userId, UpdateDisplayNameRequest request);
}

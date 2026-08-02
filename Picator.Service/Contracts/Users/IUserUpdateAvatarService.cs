using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Users;

namespace Picator.Service.Contracts.Users;

public interface IUserUpdateAvatarService
{
    Task<ApiResult> UpdateAvatar(string userId, UpdateAvatarRequest request);
}

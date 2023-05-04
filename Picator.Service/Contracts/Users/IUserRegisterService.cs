using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Users;

namespace Picator.Service.Contracts.Users;

public interface IUserRegisterService
{
    Task<ApiResult> Register(RegisterUserRequest request);
}
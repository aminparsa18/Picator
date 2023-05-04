using Picator.Common.Data.Dtos.Api.Auth;
using Picator.Common.Data.Dtos.Users;

namespace Picator.Service.Contracts.Users;

public interface IUserLoginService
{
    Task<AuthResult> Login(UserLoginRequest request);
}
using Picator.Common.Data.Dtos.Api.Auth;
using Picator.Common.Data.Dtos.Users;

namespace Picator.Service.Contracts.Users;

public interface IUserConfirmService
{
    Task<AuthResult> Confirm(ConfirmEmailRequest request);
}
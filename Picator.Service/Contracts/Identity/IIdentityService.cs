using Picator.Common.Data.Dtos.Users;

namespace Picator.Service.Contracts.Identity;

public interface IIdentityService
{
    Task<IEnumerable<ValidateUserResult>> GetByUsername(string username);
}
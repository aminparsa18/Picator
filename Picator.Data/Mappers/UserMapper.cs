using Picator.Common.Data.Dtos.Users;
using Picator.Entities.Identity;
using Riok.Mapperly.Abstractions;

namespace Picator.Data.Mappers;

[Mapper]
public partial class UserMapper
{
    public partial User RegisterRequestToUser(RegisterUserRequest request);
}
using Picator.Common.Data.Dtos.Avatars;

namespace Picator.Service.Contracts.Avatars;

public interface IAvatarService
{
    Task<IEnumerable<AvatarResult>> GetAll();
}
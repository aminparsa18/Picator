using Picator.Common.Data.Dtos.Avatars;
using Picator.Entities.Models;

namespace Picator.Repository.Contracts;

/// <summary>
/// Repository provides methods to retrieve/handle avatar data.
/// </summary>
public interface IAvatarRepository : IBaseRepository<Avatar>
{
    /// <summary>
    /// Retrieves all avatars data.
    /// </summary>
    /// <returns>List of avatars</returns>
    Task<List<AvatarResult>> GetAllDtos();
}
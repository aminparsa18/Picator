using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Avatars;

namespace Picator.Game.Services.Avatars;

/// <summary>
/// API service provides methods to retrieve/handle avatars.
/// </summary>
public interface IAvatarsApiService
{
    /// <summary>
    /// Retrieves all avatars data.
    /// </summary>
    /// <returns></returns>
    Task<ApiResult<IEnumerable<AvatarResult>>> GetAllAvatars();
}
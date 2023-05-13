using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Avatars;
using Picator.Game;
using Picator.Game.Constants;
using Picator.Game.Extensions;
using Picator.Game.Services.Avatars;
using System.Collections.Generic;

namespace Mafiator.Common.Client.Services.Avatars;

/// <inheritdoc/>
public class AvatarsApiService : IAvatarsApiService
{
    /// <inheritdoc/>
    public Task<ApiResult<IEnumerable<AvatarResult>>> GetAllAvatars()
    {
        return BaseHttpClient.Instance.GetFromMemoryPackAsync<ApiResult<IEnumerable<AvatarResult>>>(
           new Uri($"{UrlConstants.BaseUrl}avatars"));
    }
}
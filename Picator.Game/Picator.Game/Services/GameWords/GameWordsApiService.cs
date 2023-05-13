using Picator.Common.Data.Dtos.Api;
using Picator.Game.Constants;
using Picator.Game.Extensions;
using System.Collections.Generic;

namespace Picator.Game.Services.GameWords;

/// <inheritdoc/>
public class GameWordsApiService : IGameWordsApiService
{
    /// <inheritdoc/>
    public Task<ApiResult<List<string>>> GetRandomWords()
    {
        return BaseHttpClient.Instance.GetFromMemoryPackAsync<ApiResult<List<string>>>(
           new Uri($"{UrlConstants.BaseUrl}gamewords"));
    }
}
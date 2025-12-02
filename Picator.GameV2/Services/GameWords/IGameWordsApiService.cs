using Picator.Common.Data.Dtos.Api;

namespace Picator.Game.Services.GameWords;

/// <summary>
/// API service provides methods to retrieve/handle game words.
/// </summary>
public interface IGameWordsApiService
{
    /// <summary>
    /// Retrieves random game words data.
    /// </summary>
    /// <returns></returns>
    Task<ApiResult<List<string>>> GetRandomWords();
}
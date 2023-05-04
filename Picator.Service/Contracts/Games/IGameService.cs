using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Games;

namespace Picator.Service.Contracts.Games;

public interface IGameService
{
    Task<ApiResult<IEnumerable<AvailableGameResult>>> GetAvailable();

    Task<ApiResult<string>> IsJoined(string userId, string gameId);
}
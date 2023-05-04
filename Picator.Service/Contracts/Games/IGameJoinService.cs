using Picator.Common.Data.Dtos.Api;

namespace Picator.Service.Contracts.Games;

public interface IGameJoinService
{
    Task<ApiResult> Join(string userId, string gameId);
}
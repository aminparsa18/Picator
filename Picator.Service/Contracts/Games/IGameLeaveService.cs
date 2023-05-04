using Picator.Common.Data.Dtos.Api;

namespace Picator.Service.Contracts.Games;

public interface IGameLeaveService
{
    Task<ApiResult> Leave(string userId, string gameId);
}
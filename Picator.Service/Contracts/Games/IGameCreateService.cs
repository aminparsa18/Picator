using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Games;

namespace Picator.Service.Contracts.Games;

public interface IGameCreateService
{
    Task<ApiResult<GameCreateResult>> Create(GameCreateRequest request);
    Task<string?> CreateTimeGame(string gameCode);
}
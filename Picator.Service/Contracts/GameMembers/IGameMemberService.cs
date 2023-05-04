using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.GameMembers;

namespace Picator.Service.Contracts.GameMembers;

public interface IGameMemberService
{
    Task<ApiResult<IEnumerable<GameMemberResult>>> GetByGame(string gameId);
}
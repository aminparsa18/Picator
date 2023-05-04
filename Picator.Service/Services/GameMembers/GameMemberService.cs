using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.GameMembers;
using Picator.Repository;
using Picator.Service.Contracts.GameMembers;

namespace Picator.Service.Services.GameMembers;

public class GameMemberService : IGameMemberService
{
    private readonly IUnitOfWork _unitOfWork;

    public GameMemberService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResult<IEnumerable<GameMemberResult>>> GetByGame(string gameId)
    {
        var members = await _unitOfWork.GameMember.GetByGameFast(Guid.Parse(gameId));
        foreach (var gameMemberDto in members)
        {
            gameMemberDto.Avatar = string.Join(Data.Constants.BlobStorageEndpoint, gameMemberDto.Avatar);
        }
        return new ApiResult<IEnumerable<GameMemberResult>>
        {
            IsSuccess = true,
            Data = members
        };
    }
}
using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Games;
using Picator.Repository;
using Picator.Service.Contracts.Games;

namespace Picator.Service.Services.Games;

public class GameService : IGameService
{
    private readonly IUnitOfWork _unitOfWork;

    public GameService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResult<IEnumerable<AvailableGameResult>>> GetAvailable()
    {
        var data = await _unitOfWork.Game.GetAvailables();
        return new ApiResult<IEnumerable<AvailableGameResult>>
        {
            IsSuccess = true,
            Data = data
        };
    }

    public async Task<ApiResult<string>> IsJoined(string userId, string gameId)
    {
        var member = await _unitOfWork.Game.IsJoinedFast(userId, gameId);
        return new ApiResult<string>()
        {
            IsSuccess = true,
            Data = member
        };
    }
}
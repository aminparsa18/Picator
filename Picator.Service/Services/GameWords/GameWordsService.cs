using Picator.Common.Data.Dtos.Api;
using Picator.Repository;
using Picator.Service.Contracts.GameWords;

namespace Picator.Service.Services.GameWords;

public class GameWordsService : IGameWordsService
{
    private readonly IUnitOfWork _unitOfWork;

    public GameWordsService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResult<List<string>>> GetRandomWords()
    {
        return new ApiResult<List<string>>
        {
            IsSuccess = true,
            Data = await _unitOfWork.GameWord.GetRandomWords()
        };
    }
}
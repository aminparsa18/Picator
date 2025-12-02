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

    public async Task<ApiResult<string>> GetRandomWord()
    {
        var randomIndex = new Random().Next(0, 150);
        return new ApiResult<string>
        {
            IsSuccess = true,
            Data = await _unitOfWork.GameWord.GetRandomWord(randomIndex)
        };
    }
}
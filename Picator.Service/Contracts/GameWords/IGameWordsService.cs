using Picator.Common.Data.Dtos.Api;

namespace Picator.Service.Contracts.GameWords;

public interface IGameWordsService
{
    Task<ApiResult<string>> GetRandomWord();
}
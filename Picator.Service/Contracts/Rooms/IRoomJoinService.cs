using Picator.Common.Data.Dtos.Api;

namespace Picator.Service.Contracts.Rooms;

public interface IRoomJoinService
{
    Task<ApiResult<string>> JoinByCode(Guid userId, string code);

    Task<ApiResult> JoinById(Guid userId, Guid id);
}
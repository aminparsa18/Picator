using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Rooms;

namespace Picator.Service.Contracts.Rooms;

public interface IRoomCreateService
{
    Task<ApiResult<RoomCreateResult>> Create(string userId, RoomCreateRequest request);
}
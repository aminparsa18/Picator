using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Rooms;

namespace Picator.Service.Contracts.Rooms;

public interface IRoomUpdateService
{
    Task<ApiResult> Update(string userId, UpdateRoomNameRequest request);
}
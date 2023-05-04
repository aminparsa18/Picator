using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Rooms;

namespace Picator.Service.Contracts.Rooms;

public interface IRoomLeaveService
{
    Task<ApiResult> Leave(string userId, LeaveRoomRequest request);
}
using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Rooms;

namespace Picator.Service.Contracts.Rooms;

public interface IRoomService
{
    Task<ApiResult<RoomDetailsResult>> GetDetails(Guid roomId);

    Task<ApiResult<IEnumerable<RoomDetailsResult>>> GetMyRooms(Guid userId);

    Task<ApiResult<string>> IsJoined(Guid userId, Guid roomId);
}
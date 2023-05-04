using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.RoomMembers;

namespace Picator.Service.Contracts.RoomMembers;

public interface IRoomMemberService
{
    Task<ApiResult<IEnumerable<RoomMemberResult>>> GetByRoom(Guid roomId);
}
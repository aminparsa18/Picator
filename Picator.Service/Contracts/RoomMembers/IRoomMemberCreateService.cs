using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.RoomMembers;

namespace Picator.Service.Contracts.RoomMembers;

public interface IRoomMemberCreateService
{
    Task<ApiResult> Create(NewMembersRequest request);
}
using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.RoomMembers;
using Picator.Repository;
using Picator.Service.Contracts.RoomMembers;

namespace Picator.Service.Services.RoomMembers;

public class RoomMemberService : IRoomMemberService
{
    private readonly IUnitOfWork _unitOfWork;

    public RoomMemberService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResult<IEnumerable<RoomMemberResult>>> GetByRoom(Guid roomId)
    {
        var data = await _unitOfWork.RoomMember.GetByRoomFast(roomId);
        foreach (var item in data)
        {
            item.Level = (item.TotalGame / 10) + 1;
            item.Avatar = string.Join(Data.Constants.BlobStorageEndpoint, item.Avatar);
        }

        return new ApiResult<IEnumerable<RoomMemberResult>>
        {
            Data = data,
            IsSuccess = true
        };
    }
}
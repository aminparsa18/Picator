using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.RoomMembers;
using Picator.Repository;
using Picator.Service.Contracts.RoomMembers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            item.Image = string.Join(Data.Constants.BlobStorageEndpoint, item.Image);
        }

        return new ApiResult<IEnumerable<RoomMemberResult>>
        {
            Data = data,
            IsSuccess = true
        };
    }
}
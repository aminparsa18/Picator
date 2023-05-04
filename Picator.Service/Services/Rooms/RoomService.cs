using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Rooms;
using Picator.Repository;
using Picator.Service.Contracts.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Picator.Service.Services.Rooms;

public class RoomService : IRoomService
{
    private readonly IUnitOfWork _unitOfWork;

    public RoomService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResult<RoomDetailsResult>> GetDetails(Guid roomId)
    {
        var data = await _unitOfWork.Room.GetRoomFast(roomId);
        if (!data.Any())
        {
            return new ApiResult<RoomDetailsResult>
            {
                IsSuccess = false,
                StatusCode = ApiResultStatusCode.NotFound,
                Errors = new[] { "Room not found." }
            };
        }

        return new ApiResult<RoomDetailsResult>
        {
            IsSuccess = true,
            Data = data.FirstOrDefault()
        };
    }

    public async Task<ApiResult<IEnumerable<RoomDetailsResult>>> GetMyRooms(Guid userId)
    {
        var data = await _unitOfWork.Room.GetMyRoomsFast(userId);
        return new ApiResult<IEnumerable<RoomDetailsResult>>
        {
            IsSuccess = true,
            Data = data
        };
    }

    public async Task<ApiResult<string>> IsJoined(Guid userId, Guid roomId)
    {
        var member = await _unitOfWork.Room.IsJoinedFast(userId, roomId);
        return new ApiResult<string>
        {
            IsSuccess = true,
            Data = member?.ToString()
        };
    }
}
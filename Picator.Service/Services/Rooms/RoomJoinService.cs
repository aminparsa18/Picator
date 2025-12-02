using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Data.Dtos.Api;
using Picator.Entities.Models;
using Picator.Repository;
using Picator.Service.Contracts.Rooms;

namespace Picator.Service.Services.Rooms;

public class RoomJoinService : IRoomJoinService
{
    private readonly IUnitOfWork _unitOfWork;

    public RoomJoinService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResult<string>> JoinByCode(Guid userId, string code)
    {
        var roomId = await _unitOfWork.Room.GetByCode(code);
        if (roomId == null || roomId == Guid.Empty)
        {
            return new ApiResult<string>
            {
                IsSuccess = false,
                Errors = ["No such room exist"],
                StatusCode = ApiResultStatusCode.NotFound
            };
        }
        var member = await _unitOfWork.Room.IsJoinedFast(userId, roomId.Value);
        if (member != null)
            return new ApiResult<string>
            {
                IsSuccess = false,
                Errors = ["You are already joined"],
                StatusCode = ApiResultStatusCode.Conflict,
                Data = roomId.ToString()
            };
        await _unitOfWork.RoomMember.Add(new RoomMember()
        {
            RoomId = roomId.Value,
            UserId = userId
        });
        return new ApiResult<string>
        {
            IsSuccess = true,
            Data = roomId.ToString()
        };
    }

    public async Task<ApiResult> JoinById(Guid userId, Guid id)
    {
        await _unitOfWork.RoomMember.Add(new RoomMember()
        {
            UserId = userId,
            RoomId = id
        });
        return new ApiResult
        {
            IsSuccess = true
        };
    }
}
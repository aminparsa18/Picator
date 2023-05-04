using FluentValidation;
using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Rooms;
using Picator.Repository;
using Picator.Service.Contracts.Rooms;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Picator.Service.Services.Rooms;

public class RoomLeaveService : IRoomLeaveService
{
    private readonly IValidator<LeaveRoomRequest> _validator;
    private readonly IUnitOfWork _unitOfWork;

    public RoomLeaveService(IValidator<LeaveRoomRequest> validator, IUnitOfWork unitOfWork)
    {
        _validator = validator;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResult> Leave(string userId, LeaveRoomRequest request)
    {
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return new ApiResult
            {
                StatusCode = ApiResultStatusCode.BadRequest,
                Errors = validationResult.Errors.Select(e => e.ErrorMessage)
            };
        var member = await _unitOfWork.RoomMember.Get(r => r.UserId == Guid.Parse(userId) && r.RoomId == request.RoomId);
        if (member == null)
            return new ApiResult
            {
                IsSuccess = false,
                Errors = new[] { "You are not member of this room." },
                StatusCode = ApiResultStatusCode.BadRequest
            };
        _unitOfWork.RoomMember.Remove(member);
        await _unitOfWork.Commit();
        return new ApiResult
        {
            IsSuccess = true,
        };
    }
}
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

public class RoomUpdateService : IRoomUpdateService
{
    private readonly IValidator<UpdateRoomNameRequest> _validator;
    private readonly IUnitOfWork _unitOfWork;

    public RoomUpdateService(IValidator<UpdateRoomNameRequest> validator, IUnitOfWork unitOfWork)
    {
        _validator = validator;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResult> Update(string userId, UpdateRoomNameRequest request)
    {
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return new ApiResult
            {
                StatusCode = ApiResultStatusCode.BadRequest,
                Errors = validationResult.Errors.Select(e => e.ErrorMessage)
            };
        var room = await _unitOfWork.Room.Get(request.RoomId);
        if (room.UserId != Guid.Parse(userId))
            return new ApiResult
            {
                IsSuccess = false,
                Errors = new[] { "You don't have permission to perform this action." }
            };
        _unitOfWork.Room.Update(room);
        await _unitOfWork.Commit();
        return new ApiResult
        {
            IsSuccess = true
        };
    }
}
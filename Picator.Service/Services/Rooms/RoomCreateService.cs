using FluentValidation;
using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Rooms;
using Picator.Common.Helpers;
using Picator.Entities.Models;
using Picator.Repository;
using Picator.Service.Contracts.Rooms;

namespace Picator.Service.Services.Rooms;

public class RoomCreateService : IRoomCreateService
{
    private readonly IValidator<RoomCreateRequest> _validator;
    private readonly IUnitOfWork _unitOfWork;

    public RoomCreateService(IValidator<RoomCreateRequest> validator, IUnitOfWork unitOfWork)
    {
        _validator = validator;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResult<RoomCreateResult>> Create(string userId, RoomCreateRequest request)
    {
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return new ApiResult<RoomCreateResult>
            {
                StatusCode = ApiResultStatusCode.BadRequest,
                Errors = validationResult.Errors.Select(e => e.ErrorMessage)
            };
        Room room = null;// _mapper.Map<RoomCreateRequest, Room>(request);
        room.UserId = Guid.Parse(userId);
        room.Code = RandomHelper.CreateRandomText(8);
        var roomId = await _unitOfWork.Room.Add(room);
        await _unitOfWork.RoomMember.Add(new RoomMember()
        {
            RoomId = Guid.Parse(roomId.ToString()),
            UserId = room.UserId,
        });
        foreach (var user in request.Users)
        {
            await _unitOfWork.RoomMember.Add(new RoomMember()
            {
                RoomId = Guid.Parse(roomId.ToString()),
                UserId = user,
            });
        }
        return new ApiResult<RoomCreateResult>
        {
            IsSuccess = true,
            Data = new RoomCreateResult() { Id = room.Id }
        };
    }
}
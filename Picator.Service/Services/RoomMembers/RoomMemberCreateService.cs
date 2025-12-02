using FluentValidation;
using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Data.Dtos.Api;
using Picator.Common.Data.Dtos.RoomMembers;
using Picator.Entities.Models;
using Picator.Repository;
using Picator.Service.Contracts.RoomMembers;

namespace Picator.Service.Services.RoomMembers;

public class RoomMemberCreateService : IRoomMemberCreateService
{
    private readonly IValidator<NewMembersRequest> _validator;
    private readonly IUnitOfWork _unitOfWork;

    public RoomMemberCreateService(IValidator<NewMembersRequest> validator, IUnitOfWork unitOfWork)
    {
        _validator = validator;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResult> Create(NewMembersRequest request)
    {
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return new ApiResult
            {
                StatusCode = ApiResultStatusCode.BadRequest,
                Errors = validationResult.Errors.Select(e => e.ErrorMessage)
            };
        foreach (var userId in request.Users)
        {
            var user = await _unitOfWork.RoomMember.FindInRoom(request.RoomId, userId);
            if (!user.Any())
            {
                await _unitOfWork.RoomMember.Add(new RoomMember()
                {
                    UserId = userId,
                    RoomId = request.RoomId,
                });
            }
        }
        return new ApiResult
        {
            IsSuccess = true,
        };
    }
}
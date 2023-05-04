using FluentValidation;
using Picator.Common.Data.Dtos.Rooms;

namespace Picator.Service.Validations.Rooms;

public class LeaveRoomRequestValidator : AbstractValidator<LeaveRoomRequest>
{
    public LeaveRoomRequestValidator()
    {
        RuleFor(x=>x.RoomId).NotEqual(Guid.Empty);
    }
}
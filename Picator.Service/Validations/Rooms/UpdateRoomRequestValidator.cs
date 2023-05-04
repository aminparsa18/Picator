using FluentValidation;
using Picator.Common.Data.Dtos.Rooms;

namespace Picator.Service.Validations.Rooms;

public class UpdateRoomRequestValidator : AbstractValidator<UpdateRoomNameRequest>
{
    public UpdateRoomRequestValidator()
    {
        RuleFor(x => x.RoomId).NotEqual(Guid.Empty);
        RuleFor(x => x.Name).NotEmpty();
    }
}
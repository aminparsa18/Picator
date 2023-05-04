using FluentValidation;
using Picator.Common.Data.Dtos.Rooms;

namespace Picator.Service.Validations.Rooms;

public class RoomCreateResultValidator : AbstractValidator<RoomCreateRequest>
{
    public RoomCreateResultValidator()
    {
        RuleFor(x=>x.Name).NotEmpty();
        RuleFor(x => x.Users).ForEach(u => u.NotEqual(Guid.Empty));
    }
}
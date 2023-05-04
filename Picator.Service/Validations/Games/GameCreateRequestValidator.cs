using FluentValidation;
using Picator.Common.Data.Dtos.Games;

namespace Picator.Service.Validations.Games;

public class GameCreateRequestValidator : AbstractValidator<GameCreateRequest>
{
    public GameCreateRequestValidator()
    {
        RuleFor(x => x.RoomId).NotEqual(Guid.Empty);
    }
}
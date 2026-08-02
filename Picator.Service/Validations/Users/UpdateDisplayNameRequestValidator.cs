using FluentValidation;
using Picator.Common.Data.Dtos.Users;

namespace Picator.Service.Validations.Users;

public class UpdateDisplayNameRequestValidator : AbstractValidator<UpdateDisplayNameRequest>
{
    public UpdateDisplayNameRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}

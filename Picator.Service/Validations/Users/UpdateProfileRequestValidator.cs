using FluentValidation;
using Picator.Common.Data.Dtos.Users;

namespace Picator.Service.Validations.Users;

public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
{
    public UpdateProfileRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}
using FluentValidation;
using Picator.Common.Data.Dtos.Users;

namespace Picator.Service.Validations.Users;

public class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
{
    public RegisterUserRequestValidator()
    {
        RuleFor(x => x.Username).NotEmpty();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.PhoneNumber).NotEmpty();
        RuleFor(x => x.CountryCode).NotEmpty().MaximumLength(2);
    }
}
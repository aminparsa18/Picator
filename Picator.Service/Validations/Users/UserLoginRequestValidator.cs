using FluentValidation;
using Picator.Common.Data.Dtos.Users;

namespace Picator.Service.Validations.Users;

public class UserLoginRequestValidator : AbstractValidator<UserLoginRequest>
{
    public UserLoginRequestValidator()
    {
        RuleFor(x=> x.Username).NotEmpty();
        RuleFor(x=> x.Password).NotEmpty().MinimumLength(6);
    }
}
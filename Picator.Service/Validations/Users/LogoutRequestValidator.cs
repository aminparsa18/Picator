using FluentValidation;
using Picator.Common.Data.Dtos.Api.Auth;

namespace Picator.Service.Validations.Users;

public class LogoutRequestValidator : AbstractValidator<LogoutRequest>
{
    public LogoutRequestValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}

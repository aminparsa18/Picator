using FluentValidation;
using Picator.Common.Data.Dtos.Api.Auth;

namespace Picator.Service.Validations.Users;

public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
        RuleFor(x => x.Token).NotEmpty();
    }
}
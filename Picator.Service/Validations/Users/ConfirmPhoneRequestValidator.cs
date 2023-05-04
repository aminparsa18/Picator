using FluentValidation;
using Picator.Common.Data.Dtos.Users;

namespace Picator.Service.Validations.Users;

public class ConfirmPhoneRequestValidator : AbstractValidator<ConfirmPhoneRequest>
{
    public ConfirmPhoneRequestValidator()
    {
        RuleFor(x => x.PhoneNo).NotEmpty();
        RuleFor(x => x.Token).NotEmpty().Length(6);
    }
}
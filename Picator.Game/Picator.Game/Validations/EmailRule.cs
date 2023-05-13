using System.ComponentModel.DataAnnotations;

namespace Picator.Game.Validations;

public class EmailRule : IValidationRule<string>
{
    public EmailRule()
    {
        ValidationMessage = "Phải là một địa chỉ email";
    }

    public string ValidationMessage { get; set; }

    public bool Check(string value) => new EmailAddressAttribute().IsValid(value);
}
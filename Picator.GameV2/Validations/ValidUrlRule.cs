using System.ComponentModel.DataAnnotations;

namespace Picator.Game.Validations;

public class ValidUrlRule : IValidationRule<string>
{
    public ValidUrlRule()
    {
        ValidationMessage = "Phải là một URL";
    }

    public string ValidationMessage { get; set; }

    public bool Check(string value) => new UrlAttribute().IsValid(value);
}
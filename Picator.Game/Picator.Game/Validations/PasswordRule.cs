using System.Linq;

namespace Picator.Game.Validations;

public class PasswordRule<T> : IValidationRule<T>
{
    public string ValidationMessage { get; set; }

    public bool Check(T value)
    {
        if (value == null)
            return false;

        return value.ToString().Any(char.IsDigit) &&
               value.ToString().Any(char.IsLetter) &&
               value.ToString().Length > 5;
    }

    public PasswordRule()
    {
        ValidationMessage = "InvalidPassword";
    }
}
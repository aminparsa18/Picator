namespace Picator.Game.Validations;

public class IsNotNullOrEmptyRule<T> : IValidationRule<T>
{
    public IsNotNullOrEmptyRule()
    {
        ValidationMessage = string.Empty;// LocalizationResourceManager.Current["EmptyInput"];
    }

    public string ValidationMessage { get; set; }

    public bool Check(T value)
    {
        return value switch
        {
            null => false,
            //var str = value as string;
            string str => !string.IsNullOrWhiteSpace(str),
            _ => true
        };
    }
}
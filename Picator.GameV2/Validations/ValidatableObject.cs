namespace Picator.Game.Validations;

public partial class ValidatableObject<T> : ObservableObject, IValidity
{
    [ObservableProperty]
    private List<string> _errors;

    [ObservableProperty]
    private T _value;

    [ObservableProperty]
    private bool _isValid;

    public List<IValidationRule<T>> Validations { get; }

    public ValidatableObject()
    {
        _isValid = true;
        _errors = new List<string>();
        Validations = new List<IValidationRule<T>>();
    }

    public bool Validate()
    {
        var errors = Validations.Where(v => !v.Check(Value)).Select(v => v.ValidationMessage);
        Errors = errors.ToList();
        IsValid = !Errors.Any();
        return IsValid;
    }
}
namespace Picator.Game.Validations;

public interface IValidationRule<in T>
{
    string ValidationMessage { get; set; }
    bool Check(T value);
}
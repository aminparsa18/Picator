namespace Picator.Game.Controls;

/// <summary>
/// This class is used to add icon to the Entry control
/// </summary>
public class ExtendedEntry : Entry
{
    public ExtendedEntry()
    {
    }

    public static readonly BindableProperty IconProperty = BindableProperty.Create(nameof(Icon), typeof(string), typeof(ExtendedEntry), string.Empty);
  
    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
}
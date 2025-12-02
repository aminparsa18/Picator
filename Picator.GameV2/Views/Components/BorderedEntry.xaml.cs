using System.Runtime.CompilerServices;

namespace Picator.Game.Controls;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class BorderedEntry : ContentView
{
    public BorderedEntry()
    {
        InitializeComponent();
    }

    public static event EventHandler<TextChangedEventArgs> TextChanged;

    public static readonly BindableProperty PlaceholderProperty =
       BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(BorderedEntry), null, BindingMode.OneWay);

    public string Placeholder
    {
        get => GetValue(PlaceholderProperty)?.ToString();
        set => SetValue(PlaceholderProperty, value);
    }

    public static readonly BindableProperty TextProperty =
       BindableProperty.Create(nameof(Text), typeof(string), typeof(BorderedEntry), string.Empty, BindingMode.TwoWay);

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == TextProperty.PropertyName)
        {
            EntryControl.Text = Text;
        }
    }

    public string Text
    {
        get => GetValue(TextProperty)?.ToString();
        set => SetValue(TextProperty, value);
    }

    public static readonly BindableProperty IsPasswordProperty =
       BindableProperty.Create(nameof(IsPassword), typeof(bool), typeof(BorderedEntry), false);

    public bool IsPassword
    {
        get => (bool)GetValue(IsPasswordProperty);
        set => SetValue(IsPasswordProperty, value);
    }

    public static readonly BindableProperty KeyboardProperty =
      BindableProperty.Create(nameof(Keyboard), typeof(Keyboard), typeof(BorderedEntry), Keyboard.Default);

    public Keyboard Keyboard
    {
        get => (Keyboard)GetValue(KeyboardProperty);
        set => SetValue(KeyboardProperty, value);
    }

    public static readonly BindableProperty MaxLengthProperty =
      BindableProperty.Create(nameof(MaxLength), typeof(int), typeof(BorderedEntry), int.MaxValue);

    public int MaxLength
    {
        get => (int)GetValue(MaxLengthProperty);
        set => SetValue(MaxLengthProperty, value);
    }

    public static readonly BindableProperty IsValidProperty =
        BindableProperty.Create(nameof(IsValid), typeof(bool), typeof(BorderedEntry), true, propertyChanged: IsValidChanged);

    private static void IsValidChanged(BindableObject bindable, object oldValue, object newValue)
    {
        // implement when is not valid
        //  ((ExtendedEntry)bindable).FindByName<Border>(nameof(EntryBorder)).Stroke = (bool)newValue ? Colors.Green : Colors.Red;
    }

    public bool IsValid
    {
        get => (bool)GetValue(IsValidProperty);
        set => SetValue(IsValidProperty, value);
    }

    public List<string> Errors
    {
        get => (List<string>)GetValue(ErrorsProperty);
        set => SetValue(ErrorsProperty, value);
    }

    public static readonly BindableProperty ErrorsProperty =
        BindableProperty.Create(nameof(Errors), typeof(List<string>), typeof(BorderedEntry), new List<string>(), propertyChanged: ErrorsPropertyChanged);

    private static void ErrorsPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var errors = (List<string>)newValue;
        var errorLabel = ((BorderedEntry)bindable).FindByName<Label>(nameof(ErrorLabel));
        if (errors.Any())
        {
            errorLabel.IsVisible = true;
            errorLabel.Text = errors[0];
        }
        else
            errorLabel.IsVisible = false;
    }

    private void EntryControl_Focused(object sender, FocusEventArgs e)
    {
        PlaceHolderLabel.ScaleTo(0.8, 250, Easing.Linear);
        PlaceHolderLabel.TranslateTo(-45, -25, 250, Easing.CubicIn);
    }

    private void EntryControl_Unfocused(object sender, FocusEventArgs e)
    {
        if (!string.IsNullOrEmpty(EntryControl.Text))
            return;
        PlaceHolderLabel.ScaleTo(1, 250, Easing.Linear);
        PlaceHolderLabel.TranslateTo(0, 0, 250, Easing.CubicIn);
    }

    private void EntryControl_TextChanged(object sender, TextChangedEventArgs e)
    {
        Text = e.NewTextValue;
    }

    private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
    {
        EntryControl.Focus();
    }
}
namespace Picator.Game.Controls;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class ShareButton : ContentView
{
    public ShareButton()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(
            nameof(Command),
            typeof(IRelayCommand),
            typeof(ShareButton));

    public IRelayCommand? Command
    {
        get => (IRelayCommand?)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }
}
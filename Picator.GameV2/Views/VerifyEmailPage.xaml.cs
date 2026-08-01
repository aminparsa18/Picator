using Picator.Game.ViewModels;

namespace Picator.Game.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class VerifyEmailPage : ContentPage
{
    public VerifyEmailPage()
    {
        InitializeComponent();
        BindingContext = new VerifyEmailViewModel();
    }
}

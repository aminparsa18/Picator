using Rg.Plugins.Popup.Pages;
using Xamarin.Forms.Xaml;

namespace Picator.Game.Views.Popups;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class WaitingView : PopupPage
{
    public WaitingView(string message)
    {
        InitializeComponent();
        MessageLbl.Text = message;
    }
}
using Xamarin.Forms.Xaml;

namespace Picator.Game.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class StartNewGamePage : ContentPage
{
    public StartNewGamePage()
    {
        InitializeComponent();
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        //Application.Current.MainPage.Navigation.PushAsync(new GamePage(true));
    }

    private async void Button_Clicked_1(object sender, EventArgs e)
    {
        var allowed = await BarcodeScanner.Mobile.Methods.AskForRequiredPermission();
        if (allowed)
            await Application.Current.MainPage.Navigation.PushModalAsync(new BarcodeScannerView());
    }
}
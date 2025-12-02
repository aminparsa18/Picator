using Picator.Game.ViewModels;
using Picator.GameV2.Views;
using System.Threading.Tasks;

namespace Picator.Game.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class StartNewGamePage : ContentPage
{
    public StartNewGamePage()
    {
        InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (BindingContext is StartNewGameViewModel vm)
        {
            await vm.OnNavigatedToAsync();
        }
    }

    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        base.OnNavigatedFrom(args);

        if (BindingContext is StartNewGameViewModel vm)
        {
            vm.Dispose();
        }
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        Application.Current.MainPage.Navigation.PushAsync(new GamePage(true, "sdfsdfsdf"));
    }

    private void Button_Clicked_1(object sender, EventArgs e)
    {
    }

    private async void Button_Clicked_2(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new GameCodeScanner());
    }
}
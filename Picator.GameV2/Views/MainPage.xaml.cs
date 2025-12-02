using Picator.Game.Cache;

namespace Picator.Game.Views;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        if(Barrel.Current.Exists("InvitationCode"))
        {
            await Navigation.PushAsync(new GamePage(true, Barrel.Current.Get<string>("InvitationCode")));
            Barrel.Current.Empty("InvitationCode");
        }
    }
}
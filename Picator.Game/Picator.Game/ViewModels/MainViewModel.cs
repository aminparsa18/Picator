using Picator.Game.Cache;
using System.Web;

namespace Picator.Game.ViewModels;

public sealed partial class MainViewModel : ViewModelBase
{
    public MainViewModel()
    {
        var appUri = (Application.Current as App).AppUri;
        string[] segments = appUri?.Segments;
        if (segments != null && segments.Length > 1)
        {
            var path = segments[1];
            if (path.StartsWith("join-game"))
            {
                var queryDictionary = HttpUtility.ParseQueryString(appUri.Query);
                var gameCode = HttpUtility.HtmlDecode(queryDictionary["game_code"]);
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Application.Current.MainPage.Navigation.PushAsync(Barrel.Current.Exists("Token") ? new GamePage(false, gameCode) : new LoginPage());
                });
            }
        }
    }

    [RelayCommand]
    private async Task NavigateToStartGame()
    {
        await Application.Current.MainPage.Navigation.PushAsync(Barrel.Current.Exists("Token") ? new StartNewGamePage() : new LoginPage());
    }

    [RelayCommand]
    private async Task NavigateToLogin() =>
        await Application.Current.MainPage.Navigation.PushAsync(!Barrel.Current.Exists("Token") ? new ProfileSettingsPage() : new LoginPage());
}
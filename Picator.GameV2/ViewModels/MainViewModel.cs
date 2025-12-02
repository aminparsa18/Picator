using Picator.Game.Cache;
using Picator.GameV2;
using System.Web;

namespace Picator.Game.ViewModels;

public sealed partial class MainViewModel : ViewModelBase
{
    public MainViewModel()
    {
        Application.Current.Dispatcher.Dispatch(async () =>
        {


            Uri? appUri = (Application.Current as App)?.AppUri;
            string[]? segments = appUri?.Segments;
            if (segments != null && segments.Length > 1)
            {
                var path = segments[1];
                if (path.StartsWith("join-game"))
                {
                    var queryDictionary = HttpUtility.ParseQueryString(appUri?.Query);
                    string? gameCode = HttpUtility.HtmlDecode(queryDictionary["game_code"]);
                    await Application.Current.MainPage.Navigation.PushAsync(Barrel.Current.Exists("Token") ? new GamePage(false, gameCode) : new LoginPage());
                }
            }
        });
    }

    [RelayCommand]
    private async Task NavigateToStartGame()
    {
        try
        {
            await Application.Current.MainPage.Navigation.PushAsync(Barrel.Current.Exists("Token") ? new StartNewGamePage() : new StartNewGamePage());
        }
        catch (Exception ex)
        {
            var ss = 2;
        }


    }

    [RelayCommand]
    private async Task NavigateToLogin() =>
        await Application.Current.MainPage.Navigation.PushAsync(Barrel.Current.Exists("Token") ? new ProfileSettingsPage() : new LoginPage());
}
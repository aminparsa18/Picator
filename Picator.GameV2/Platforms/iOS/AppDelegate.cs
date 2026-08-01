using Foundation;
using Microsoft.Maui;
using Picator.Game;
using Picator.Game.Cache;
using System.Net.Http.Headers;
using System.Web;
using UIKit;

namespace Picator.GameV2;
[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
    {
        if (url.Host != "ec.pctor")
            return base.OpenUrl(app, url, options);

        HandleEmailConfirmation(url);
        return true;
    }

    private static void HandleEmailConfirmation(NSUrl url)
    {
        var uri = new Uri(url.AbsoluteString!);
        var query = HttpUtility.ParseQueryString(uri.Query);
        var isSuccess = string.Equals(query["isSuccess"], "True", StringComparison.OrdinalIgnoreCase);
        var errors = query["errors"];

        if (isSuccess)
        {
            var token = query["token"];
            var refreshToken = query["refresh_token"];
            Barrel.Current.Add("Token", token, TimeSpan.FromMinutes(20));
            Barrel.Current.Add("RefreshToken", refreshToken, TimeSpan.FromDays(150));
            BaseHttpClient.Instance.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            if (isSuccess)
                await Shell.Current.GoToAsync("//main");
            else
                await Microsoft.Maui.Controls.Application.Current!.MainPage!.DisplayAlert("Email confirmation failed", errors, "OK");
        });
    }
}

using Microsoft.Windows.AppLifecycle;
using System.Web;
using Windows.ApplicationModel.Activation;
using Picator.Game.ViewModels;

namespace Picator.GameV2.Platforms.Windows;

public class ProtocolHandler
{
    public static void Register()
    {
        AppInstance.GetCurrent().Activated += OnActivated;
    }

    private static void OnActivated(object? sender, AppActivationArguments args)
    {
        if (args.Kind == ExtendedActivationKind.Protocol)
        {
            var data = args.Data as IProtocolActivatedEventArgs;
            if (data?.Uri?.AbsoluteUri?.StartsWith("picator://auth") == true)
            {
                var query = HttpUtility.ParseQueryString(data.Uri.Query);
                var token = query["token"];
                var refreshToken = query["refresh_token"];
                var email = query["email"];
                var name = query["name"];

                var result = new LoginViewModel.ExternalAuthResult
                {
                    Token = token,
                    RefreshToken = refreshToken,
                    Email = email,
                    Name = name
                };

                LoginViewModel.ExternalAuthService.HandleExternalAuthResult(result);
            }
        }
    }
} 
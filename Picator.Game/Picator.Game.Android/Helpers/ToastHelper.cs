using Picator.Game.Droid.Helpers;
using Picator.Game.Services;
using Xamarin.Essentials;

[assembly: Xamarin.Forms.Dependency(typeof(ToastHelper))]
namespace Picator.Game.Droid.Helpers
{
    public class ToastHelper : IAlert
    {
        public void Show(string message, MessageType type)
        {
            switch (type)
            {
                case MessageType.None:
                    Toasty.Normal(Platform.AppContext, message).Show();
                    break;
                    ;
                case MessageType.Success:
                    Toasty.Success(Platform.AppContext, message).Show();
                    break;
                    ;
                case MessageType.Error:
                    Toasty.Error(Platform.AppContext, message).Show();
                    break;
                    ;
                case MessageType.Warning:
                    Toasty.Warning(Platform.AppContext, message).Show();
                    break;
                    ;
                case MessageType.Info:
                    Toasty.Info(Platform.AppContext, message).Show();
                    break;
                    ;
            }
        }
    }
}
using Picator.Game.Cache;
using Picator.GameV2.Services.Audio;

namespace Picator.GameV2;

public partial class AppShell : Shell
{
    private bool _hasCheckedAuth;

    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("verifyemail", typeof(VerifyEmailPage));
        ButtonSoundHook.Attach(this);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_hasCheckedAuth)
            return;
        _hasCheckedAuth = true;

        if (Barrel.Current.Exists("Token"))
            await GoToAsync("//main");
    }
}

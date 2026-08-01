using Picator.Game.Cache;

namespace Picator.GameV2;

public partial class AppShell : Shell
{
    private bool _hasCheckedAuth;

    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("verifyemail", typeof(VerifyEmailPage));
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

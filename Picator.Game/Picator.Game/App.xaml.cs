using MarcTron.Plugin;
using Picator.Game.Cache;
using Picator.Game.Resources.Text;
using Xamarin.CommunityToolkit.Helpers;

namespace Picator.Game;

public partial class App : Application
{
    public Uri? AppUri { get; }

    public App(Uri? uri = null)
    {
        AppUri = uri;
        InitializeComponent();
        Barrel.ApplicationId = "safsdfy876";
        LocalizationResourceManager.Current.Init(AppResource.ResourceManager);
        //CrossMTAdmob.Current.UserPersonalizedAds = true;
        CrossMTAdmob.Current.ComplyWithFamilyPolicies = true;
        CrossMTAdmob.Current.UseRestrictedDataProcessing = true;
        CrossMTAdmob.Current.AdsId = "ca-app-pub-3940256099942544~3347511713";
        MainPage = new NavigationPage(new MainPage());
    }

    protected override void OnStart()
    {
    }

    protected override void OnSleep()
    {
    }

    protected override void OnResume()
    {
    }
}
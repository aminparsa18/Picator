using Picator.Game.Cache;
using Plugin.Maui.Audio;
using Plugin.MauiMtAdmob;

namespace Picator.GameV2;

public partial class App : Application
{
    private IAudioPlayer _audioPlayer;

    public Uri? AppUri { get; }

    public App(Uri? uri = null)
    {
        AppUri = uri;
        InitializeComponent();
        CancellationTokenSource source = new CancellationTokenSource();
        CancellationToken token = source.Token;
        Barrel.ApplicationId = "safsdfy876";
        // LocalizationResourceManager.Current.Init(AppResource.ResourceManager);
        CrossMauiMTAdmob.Current.UserPersonalizedAds = true;
        CrossMauiMTAdmob.Current.ComplyWithFamilyPolicies = true;
        CrossMauiMTAdmob.Current.UseRestrictedDataProcessing = true;
        CrossMauiMTAdmob.Current.BannerAdsId = "ca-app-pub-3940256099942544~3347511713";
        VersionTracking.Track();
        //AppCenter.Start("android=0391a23b-6cb0-497d-8421-fe992fccbfeb;" +
        //                "uwp=af93ceea-b293-4d4e-8882-0b311ed9828a;" +
        //                "ios={Your iOS App secret here}",
        //    typeof(Analytics), typeof(Crashes));
        //Dispatcher.DispatchAsync(async () =>
        //{
        //    using var stream = await FileSystem.OpenAppPackageFileAsync("picator.mp3");
        //    _audioPlayer = AudioManager.Current.CreatePlayer(stream);
        //    _audioPlayer.Play();
        //});
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new(new AppShell());
    }
}
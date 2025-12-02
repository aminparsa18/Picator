using Android.App;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.OS;
using MediaManager;

namespace Picator.GameV2;

[Activity(Theme = "@style/Maui.SplashTheme", ResizeableActivity = true, LaunchMode = LaunchMode.SingleTop, MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        CrossMediaManager.Current.Init(this);
        MobileAds.Initialize(this);
    }
}
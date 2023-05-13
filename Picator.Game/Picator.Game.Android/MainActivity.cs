using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.Gms.Ads.Initialization;
using Android.OS;
using Android.Runtime;
using BarcodeScanner.Mobile.Droid;
using MediaManager;
using System;

namespace Picator.Game.Droid
{
    [Activity(Label = "Picator.Game", Icon = "@mipmap/ic_launcher", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    [IntentFilter(
        new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        DataSchemes = new[] { "app" },
        DataHost = "invite.pctor")]

    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            MobileAds.Initialize(this);
            CrossMediaManager.Current.Init(this);
            Rg.Plugins.Popup.Popup.Init(this);
            RendererInitializer.Init();
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Xamarin.Forms.Forms.Init(this, savedInstanceState);
            Xamarin.Auth.Presenters.XamarinAndroid.AuthenticationConfiguration.Init(this, savedInstanceState);
            Xamarin.Auth.CustomTabsConfiguration.CustomTabsClosingMessage = null;
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(true);
            if (Intent?.Data != null)
            {
                var uri = new Uri(Intent.Data.ToString() ?? string.Empty);
                LoadApplication(new App(uri));
            }
            else
                LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
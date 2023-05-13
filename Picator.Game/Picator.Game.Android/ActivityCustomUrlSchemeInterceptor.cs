using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Picator.Game.Services.OAuth;
using System;
using Xamarin.Essentials;

namespace Picator.Game.Droid
{
    [Activity(Label = "ActivityCustomUrlSchemeInterceptor", NoHistory = true, LaunchMode = LaunchMode.SingleTop, Exported = true)]
    [IntentFilter(
        new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        DataSchemes = new[] { "https" },
        DataHost = "picatorexternalauth-app-20230511.victoriousrock-9f2ad982.centralus.azurecontainerapps.io")]
    public class ActivityCustomUrlSchemeInterceptor : WebAuthenticatorCallbackActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            global::Android.Net.Uri uri_android = Intent.Data;
            // Convert Android.Net.Url to Uri
            var uri = new Uri(uri_android.ToString());

            // Close browser 
            var intent = new Intent(this, typeof(MainActivity));
            //intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
            StartActivity(intent);

            // Load redirectUrl page
            OAuthAuthenticatorHelper.AuthenticationState.OnPageLoading(uri);

            this.Finish();
        }
    }
}
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Picator.Game;
using Picator.Game.Cache;
using System.Net.Http.Headers;

namespace Picator.GameV2
{
    [Activity(NoHistory = true, LaunchMode = LaunchMode.SingleTop, Exported = true)]
    [IntentFilter(
        new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        DataSchemes = new[] { "app" },
        DataHost = "auth.pctor",
        AutoVerify = true)
        ]
    public class ActivityCustomUrlSchemeInterceptor : WebAuthenticatorCallbackActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Convert Android.Net.Url to Uri
            var uri = new Uri(Intent.Data.ToString());
            var dic = uri.ParseQueryString();
            Barrel.Current.Add("Token", Uri.UnescapeDataString(dic["token"]), TimeSpan.FromMinutes(20));
            Barrel.Current.Add("RefreshToken", Uri.UnescapeDataString(dic["refresh_token"]), TimeSpan.FromDays(150));
            Barrel.Current.Add("AccountHasName", Convert.ToBoolean(dic["has_name"]), TimeSpan.FromSeconds(20));
            BaseHttpClient.Instance.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", Uri.UnescapeDataString(dic["token"]));
            // Close browser 
            var intent = new Intent(this, typeof(MainActivity));
           // intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
            StartActivity(intent);
            this.Finish();
        }
    }
}
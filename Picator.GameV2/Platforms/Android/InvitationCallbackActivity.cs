using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Picator.Game.Cache;

namespace Picator.GameV2.Platforms.Android;

[Activity(
        NoHistory = true,
        LaunchMode = LaunchMode.SingleTop,
        Exported = true)]
[IntentFilter(
        [Intent.ActionView],
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        DataScheme = "app",
        DataHost = "inv.pctor",
        DataPathPrefix = "/invitation",
        AutoVerify = true)]
public class InvitationCallbackActivity : Activity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Get the intent data
        var uri = Intent?.Data;

        if (uri != null)
        {
            // Extract parameters from the deep link
            var code = uri.GetQueryParameter("code");
            if(!string.IsNullOrEmpty(code))
            {
                Barrel.Current.Add("InvitationCode", code, TimeSpan.FromDays(1));
            }
        }

        // Close browser 
        var intent = new Intent(this, typeof(MainActivity));
        // intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
        StartActivity(intent);
        this.Finish();
    }
}
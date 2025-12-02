using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using Picator.Game;
using Picator.Game.Cache;
using System.Net.Http.Headers;

namespace Picator.GameV2.Platforms.Android;

[Activity(
        NoHistory = true,
        LaunchMode = LaunchMode.SingleTop,
        Exported = true)]
[IntentFilter(
        [Intent.ActionView],
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        DataScheme = "app",
        DataHost = "ec.pctor",
        DataPathPrefix = "/email-confirmation",
        AutoVerify = true)]
public class EmailConfirmationCallbackActivity : Activity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Get the intent data
        var uri = Intent?.Data;

        if (uri != null)
        {
            // Extract parameters from the deep link
            var isSuccess = string.Equals(uri.GetQueryParameter("isSuccess"), "True", StringComparison.InvariantCultureIgnoreCase);
            var token = uri.GetQueryParameter("token");
            var refreshToken = uri.GetQueryParameter("refresh_token");
            var errors = uri.GetQueryParameter("errors");

            // Handle the confirmation result
            HandleEmailConfirmation(isSuccess, token, refreshToken, errors);
        }

        // Close this activity and return to the app
        Finish();
    }

    private void HandleEmailConfirmation(bool isSuccess, string token, string refreshToken, string errors)
    {
        if (isSuccess)
        {
            // Show success message
            Toast.MakeText(this, "Email confirmed successfully!", ToastLength.Long)?.Show();

            Barrel.Current.Add("Token", token, TimeSpan.FromMinutes(20));
            Barrel.Current.Add("RefreshToken", refreshToken, TimeSpan.FromDays(150));
            BaseHttpClient.Instance.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
            // Close browser 
            var intent = new Intent(this, typeof(MainActivity));
            // intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
            StartActivity(intent);
            this.Finish();
        }
        else
        {
            // Show error message
            Toast.MakeText(this, $"Email confirmation failed: {errors}", ToastLength.Long)?.Show();

            // Close browser 
            var intent = new Intent(this, typeof(MainActivity));
            // intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
            StartActivity(intent);
            this.Finish();
        }
    }
}
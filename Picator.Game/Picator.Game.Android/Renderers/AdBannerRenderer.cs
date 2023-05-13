using Android.Content;
using Android.Gms.Ads;
using Android.Widget;
using MafiatorApp.Droid.Renderers;
using Picator.Game.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Platform = Xamarin.Essentials.Platform;

[assembly: ExportRenderer(typeof(AdBanner), typeof(AdBannerRenderer))]
namespace MafiatorApp.Droid.Renderers
{
    public class AdBannerRenderer : ViewRenderer<AdBanner, AdView>
    {
        private string adUnitId = string.Empty;
        //Note you may want to adjust this, see further down.
        private readonly AdSize adSize = GetFullWidthAdaptiveSize();
        private AdView adView;
        public AdBannerRenderer(Context context) : base(context)
        {
        }

        private AdView CreateNativeAdControl()
        {
            if (adView != null)
                return adView;

            // This is a string in the Resources/values/strings.xml that I added or you can modify it here. This comes from admob and contains a / in it
            adUnitId = "ca-app-pub-3940256099942544/6300978111";// "ca-app-pub-5204679523132638/6212946482";
            adView = new AdView(Context) {AdSize = adSize, AdUnitId = adUnitId};

            var adParams = new LinearLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);
            adView.LayoutParameters = adParams;
            adView.LoadAd(new AdRequest.Builder().Build());
            return adView;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<AdBanner> e)
        {
            base.OnElementChanged(e);
            if (Control != null) return;
            CreateNativeAdControl();
            SetNativeControl(adView);
        }

        private static AdSize GetFullWidthAdaptiveSize()
        {
            var displayInfo = Xamarin.Essentials.DeviceDisplay.MainDisplayInfo;
            var adWidth = (int)(displayInfo.Width/ displayInfo.Density);
            return AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSize(Platform.CurrentActivity, adWidth);
        }
    }
}
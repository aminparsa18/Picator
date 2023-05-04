using MarcTron.Plugin;
using Picator.Game.Views;
using Xamarin.Forms;

namespace Picator.Game
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
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
}
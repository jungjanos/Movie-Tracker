using Android.App;
using Android.Content.PM;
using Android.OS;
using System.Net.Http;
using PanCardView.Droid;
using Xamarin.Forms;
using Xamarin.Essentials;
using FFImageLoading.Forms.Platform;
using Ch9.Views;
using Ch9.Services;

namespace Ch9.Droid
{
    [Activity(Label = "Movie Tracker", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            Forms.SetFlags("CollectionView_Experimental");
            Xamarin.Forms.Forms.Init(this, savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            CachedImageRenderer.Init(true);
            CardsViewRenderer.Preserve();

            SubscribeToMessages();

            HttpClient httpClient = new HttpClient(new Xamarin.Android.Net.AndroidClientHandler());
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows) Gecko Firefox");

            LoadApplication(new App(httpClient));
        }

        private void SubscribeToMessages()
        {
            //allowing the sender (Page) to request a fixed screen orientation
            MessagingCenter.Subscribe<VideoPage>(this, MessagingCenterMessages.SET_LANDSCAPE, sender =>
            {
                RequestedOrientation = ScreenOrientation.Landscape;
            });
            
            MessagingCenter.Subscribe<VideoPage>(this, MessagingCenterMessages.SET_PORTRAIT, sender =>
            {
                RequestedOrientation = ScreenOrientation.Portrait;
            });

            MessagingCenter.Subscribe<LargeImagePage>(this, MessagingCenterMessages.SET_LANDSCAPE, sender =>
            {
                RequestedOrientation = ScreenOrientation.Landscape;
            });

            MessagingCenter.Subscribe<LargeImagePage>(this, MessagingCenterMessages.SET_PORTRAIT, sender =>
            {
                RequestedOrientation = ScreenOrientation.Portrait;
            });
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [Android.Runtime.GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
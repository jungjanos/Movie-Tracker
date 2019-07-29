using Android.App;
using Android.Content.PM;
using Android.OS;
using System.Net.Http;
using PanCardView.Droid;
using Xamarin.Forms;

namespace Ch9.Droid
{
    [Activity(Label = "Ch9", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            Forms.SetFlags("CollectionView_Experimental");
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            CardsViewRenderer.Preserve();
            HttpClient httpClient = new HttpClient(new Xamarin.Android.Net.AndroidClientHandler());

            LoadApplication(new App(httpClient));
        }
    }
}
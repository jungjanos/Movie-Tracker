using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Ch9.Models;
using Ch9.Services;

namespace Ch9.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VideoPage : ContentPage
    {
        public VideoPage(TmdbVideoModel attachedVideo)
        {
            BindingContext = attachedVideo;
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            MessagingCenter.Send(this, MessagingCenterMessages.SET_LANDSCAPE);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Send(this, MessagingCenterMessages.SET_PORTRAIT);
        }
    }
}
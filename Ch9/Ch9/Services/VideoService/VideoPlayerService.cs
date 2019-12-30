using Ch9.Services.Contracts;
using Ch9.Views;

using Xamarin.Forms;
using System.Threading.Tasks;
using Xamarin.Essentials;
using System;

namespace Ch9.Services.VideoService
{
    public class VideoPlayerService : IPlayVideo
    {
        public async Task OpenVideoStreamDirectly(string streamUrl)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new VideoPage(streamUrl));
        }

        public async Task OpenYoutubeVideoInDeviceDefaultPlayer(string youtubeUrl)
        {
            try
            {
                await Browser.OpenAsync(youtubeUrl, BrowserLaunchMode.SystemPreferred);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Could not open Youtube Link: {ex.Message}", "Ok");
            }
        }
    }
}

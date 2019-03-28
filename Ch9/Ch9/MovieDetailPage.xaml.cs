using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Ch9.Models;
using Ch9.ApiClient;
using static Ch9.ApiClient.TheMovieDatabaseClient;

namespace Ch9
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MovieDetailPage : ContentPage
    {

        private MovieDetailModel _movie;
        private Task<GetMovieImagesResult> imageDetailCollectionUpdateTask;

        public MovieDetailPage(MovieDetailModel movie)
        {
            _movie = movie;
            imageDetailCollectionUpdateTask = ((App)App.Current).ApiClient.UpdateMovieImages(_movie);
            InitializeComponent();
            BindingContext = _movie;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            MovieDetailsUpdateResult updateResult = null;

            try
            {
                updateResult = await ((App)App.Current).movieModelDeatilUpdateGetter(_movie);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Exception", ex.Message, "OK");
            }

            if (updateResult.StatusCode != System.Net.HttpStatusCode.OK)
            {
                await DisplayAlert("Update error", $"Error code: {updateResult.StatusCode.ToString()}", "Ok");
            }
            else
            {

            }

        }

        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            Task<GetMovieImagesResult> imageDetailCollectionUpdateTask1 = imageDetailCollectionUpdateTask;
            GetMovieImagesResult updateResult;
            try
            {

                updateResult = await imageDetailCollectionUpdateTask1;
                _movie.GalleryPositionCounter++;
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.InnerExceptions) 
                    await DisplayAlert("Exception", ex.GetType().Name+ ex.Message + " " + ex.StackTrace, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Exception", ex.GetType().Name + ex.Message + " " + ex.StackTrace, "OK"); 
            }

            
        }
    }
}
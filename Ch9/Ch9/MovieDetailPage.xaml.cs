using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Ch9.Models;
using static Ch9.ApiClient.TmdbNetworkClient;
using Newtonsoft.Json;
using Ch9.ApiClient;

namespace Ch9
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MovieDetailPage : ContentPage
    {

        private MovieDetailModel _movie;        

        private Task<GetMovieImagesResult> imageDetailCollectionUpdateTask2;
        Task initializeGallery;

        private Settings settings;

        public MovieDetailPage(MovieDetailModel movie)
        {
            _movie = movie;

            // starts a hot task to fetch gallery image paths as early as possible
            settings = ((App)Application.Current).Settings;

            imageDetailCollectionUpdateTask2 = ((App)Application.Current).CachedSearchClient.UpdateMovieImages(_movie.Id, settings.SearchLanguage, null, true);

            // attaches task to update movie gallery details with the results of the antecendent
            initializeGallery = imageDetailCollectionUpdateTask2.ContinueWith(t =>
            {
                if (200 <= (int)t.Result.HttpStatusCode && (int)t.Result.HttpStatusCode < 300)
                {
                    // ToDo: initialize that in the MovieDetailModel constructor
                    if (_movie.ImageDetailCollection == null)
                        _movie.ImageDetailCollection = new ImageDetailCollection();

                    JsonConvert.PopulateObject(t.Result.Json, _movie.ImageDetailCollection);
                    ((App)Application.Current).MovieDetailModelConfigurator.SetGalleryImageSources(_movie);
                }                    
            });

            InitializeComponent();
            BindingContext = _movie;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            FetchMovieDetailsResult movieDetailsResult = await ((App)Application.Current).CachedSearchClient.FetchMovieDetails(_movie.Id, settings.SearchLanguage);
            if (200 <= (int)movieDetailsResult.HttpStatusCode && (int)movieDetailsResult.HttpStatusCode < 300)
                JsonConvert.PopulateObject(movieDetailsResult.Json, _movie);
        }

        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            await initializeGallery;
            _movie.GalleryPositionCounter++;       
        }
    }
}
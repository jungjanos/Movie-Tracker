using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Ch9.Models;
using Newtonsoft.Json;
using Ch9.ApiClient;
using Ch9.Utils;

namespace Ch9
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MovieDetailPage : ContentPage
    {
        private MovieDetailModel _movie;        
        private ISettings _settings;
        private ITmdbCachedSearchClient _cachedSearchClient;
        private Task _fetchGallery;

        public MovieDetailPage(MovieDetailModel movie)
        {
            _movie = movie;
            _settings = ((App)Application.Current).Settings;
            _cachedSearchClient = ((App)Application.Current).CachedSearchClient;

            _fetchGallery = UpdateImageDetailCollection();

            InitializeComponent();
            BindingContext = _movie;
        }

        // starts a hot task to fetch and adjust gallery image paths as early as possible        
        private Task UpdateImageDetailCollection()
        {            
            var imageDetailCollectionUpdateTask = _cachedSearchClient.UpdateMovieImages(_movie.Id, _settings.SearchLanguage, null, true);

            // attaches task to adjust movie gallery details with the results of the antecendent
            var galleryCollectionReady = imageDetailCollectionUpdateTask.ContinueWith(t =>
            {
                if (t.Result.HttpStatusCode.IsSuccessCode())
                {
                    JsonConvert.PopulateObject(t.Result.Json, _movie.ImageDetailCollection);
                    ((App)Application.Current).MovieDetailModelConfigurator.SetGalleryImageSources(_movie);
                }
            });
            return galleryCollectionReady;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            FetchMovieDetailsResult movieDetailsResult = await _cachedSearchClient.FetchMovieDetails(_movie.Id, _settings.SearchLanguage);
            if (movieDetailsResult.HttpStatusCode.IsSuccessCode())
                JsonConvert.PopulateObject(movieDetailsResult.Json, _movie);
        }

        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            await _fetchGallery;
            _movie.GalleryPositionCounter++;       
        }

        private async void OnRecommendationButton_Clicked(object sender, EventArgs e)
        {
            Task<GetMovieRecommendationsResult> getMovieRecommendations = _cachedSearchClient.GetMovieRecommendations(_movie.Id, _settings.SearchLanguage);
            Task<GetSimilarMoviesResult> getSimilarMovies = _cachedSearchClient.GetSimilarMovies(_movie.Id, _settings.SearchLanguage);

            GetMovieRecommendationsResult movieRecommendationsResult = await getMovieRecommendations;

            if (movieRecommendationsResult.HttpStatusCode.IsSuccessCode())
                await Navigation.PushAsync(new RecommendationsPage(_movie, getMovieRecommendations, getSimilarMovies));

        }

        private async void OnMainPageButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }
    }
}
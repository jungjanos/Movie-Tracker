using Ch9.ApiClient;
using Ch9.Models;
using Ch9.Utils;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Ch9
{
    public class MovieDetailPageViewModel
    {
        public MovieDetailModel Movie { get; set; }
        private readonly ISettings _settings;
        private readonly ITmdbCachedSearchClient _cachedSearchClient;
        private readonly IMovieDetailModelConfigurator _movieDetailModelConfigurator;
        private readonly IPageService _pageService;
        private readonly Task _fetchGallery;

        public ICommand HomeCommand { get; private set; }
        public ICommand RecommendationsCommand { get; private set; }
        public ICommand AddToListCommand { get; set; }
        public ICommand ImageStepCommand { get; private set; }


        public MovieDetailPageViewModel(
            MovieDetailModel movie,
            ISettings settings,
            ITmdbCachedSearchClient cachedSearchClient,
            IMovieDetailModelConfigurator movieDetailModelConfigurator,
            IPageService pageService)
        {
            Movie = movie;
            _settings = settings;
            _cachedSearchClient = cachedSearchClient;
            _movieDetailModelConfigurator = movieDetailModelConfigurator;
            _pageService = pageService;
            _fetchGallery = UpdateImageDetailCollection();
            
            ImageStepCommand = new Command(async () => { await _fetchGallery; Movie.GalleryPositionCounter++; });
            HomeCommand = new Command(async () => await _pageService.PopToRootAsync());
            RecommendationsCommand = new Command(async () => await OnRecommendationsCommand());
            AddToListCommand = new Command(async () => await OnAddToListCommand());
        }

        public async Task Initialize()
        {
            FetchMovieDetailsResult movieDetailsResult = await _cachedSearchClient.FetchMovieDetails(Movie.Id, _settings.SearchLanguage);
            if (movieDetailsResult.HttpStatusCode.IsSuccessCode())
                JsonConvert.PopulateObject(movieDetailsResult.Json, Movie);
        }

        // starts a hot task to fetch and adjust gallery image paths as early as possible        
        private Task UpdateImageDetailCollection()
        {
            var imageDetailCollectionUpdateTask = _cachedSearchClient.UpdateMovieImages(Movie.Id, _settings.SearchLanguage, null, true);

            // attaches task to adjust movie gallery details with the results of the antecendent
            var galleryCollectionReady = imageDetailCollectionUpdateTask.ContinueWith(t =>
            {
                if (t.Result.HttpStatusCode.IsSuccessCode())
                {
                    JsonConvert.PopulateObject(t.Result.Json, Movie.ImageDetailCollection);
                    _movieDetailModelConfigurator.SetGalleryImageSources(Movie);
                }
            });
            return galleryCollectionReady;
        }

        public async Task OnRecommendationsCommand()
        {
            Task<GetMovieRecommendationsResult> getMovieRecommendations = _cachedSearchClient.GetMovieRecommendations(Movie.Id, _settings.SearchLanguage);
            Task<GetSimilarMoviesResult> getSimilarMovies = _cachedSearchClient.GetSimilarMovies(Movie.Id, _settings.SearchLanguage);

            GetMovieRecommendationsResult movieRecommendationsResult = await getMovieRecommendations;

            if (movieRecommendationsResult.HttpStatusCode.IsSuccessCode())
                await _pageService.PushRecommendationsPageAsync(Movie, getMovieRecommendations, getSimilarMovies);
        }

        public async Task OnAddToListCommand()
        {

        }

    }
}

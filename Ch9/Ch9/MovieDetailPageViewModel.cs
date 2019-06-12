using Ch9.ApiClient;
using Ch9.Models;
using Ch9.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Ch9
{
    public class MovieDetailPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public MovieDetailModel Movie { get; set; }
        private readonly ISettings _settings;
        private readonly ITmdbCachedSearchClient _cachedSearchClient;
        private readonly IMovieDetailModelConfigurator _movieDetailModelConfigurator;
        private readonly IPageService _pageService;
        private readonly Task _fetchGallery;
        private readonly Task _fetchReviews;
        public ObservableCollection<Review> Reviews { get; } = new ObservableCollection<Review>(); 
        public bool? MovieIsAlreadyOnActiveList => _settings.MovieIdsOnActiveList?.Contains(Movie.Id);
        public bool? MovieHasReviews => _fetchReviews.IsCompleted ? (Reviews.Count > 0 ? true : false) : (bool?)null;
         

        public ICommand HomeCommand { get; private set; }
        public ICommand RecommendationsCommand { get; private set; }
        public ICommand ReviewsCommand { get; private set; }
        public ICommand AddToListCommand { get; private set; }
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
            _fetchReviews = UpdateReviewCollection();
            
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

        private async Task UpdateReviewCollection()
        {
            var getReviewResult = await _cachedSearchClient.GetMovieReviews(Movie.Id, language: null, page: null, retryCount: 3, delayMilliseconds: 1000, fromCache: false);
            if (getReviewResult.HttpStatusCode.IsSuccessCode())
            {
                GetReviewsModel reviewsWrapper = JsonConvert.DeserializeObject<GetReviewsModel>(getReviewResult.Json);

                foreach (Review review in reviewsWrapper.Reviews)
                {
                    if (Reviews.FirstOrDefault(r => r.Id == review.Id) == null)
                        Reviews.Add(review);
                }
            }
            OnPropertyChanged(nameof(MovieHasReviews));
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
            if (MovieIsAlreadyOnActiveList == null || _settings.ActiveMovieListId == null)
            {
                await _pageService.DisplayAlert("Info", "You have to select a list to be able to add movies to it", "Cancel");
                return;
            }                

            if (MovieIsAlreadyOnActiveList == true)
            {
                RemoveMovieResult result = await _cachedSearchClient.RemoveMovie(_settings.ActiveMovieListId.Value, Movie.Id, retryCount: 3, delayMilliseconds: 1000);

                if (result.HttpStatusCode.IsSuccessCode())
                {
                    _settings.MovieIdsOnActiveList = _settings.MovieIdsOnActiveList.Except(Enumerable.Repeat(Movie.Id, 1)).ToArray();
                    OnPropertyChanged(nameof(MovieIsAlreadyOnActiveList));                    
                }
                else
                    await _pageService.DisplayAlert("Error", "Could not remove item from active list. Try refreshing the lists first or use the TMDB webpage directly", "Ok");

                return;
            }

            if (MovieIsAlreadyOnActiveList == false)
            {
                AddMovieResult result = await _cachedSearchClient.AddMovie(_settings.ActiveMovieListId.Value, Movie.Id, retryCount: 3, delayMilliseconds: 1000);

                if (result.HttpStatusCode.IsSuccessCode())
                {
                    _settings.MovieIdsOnActiveList = _settings.MovieIdsOnActiveList.Union(Enumerable.Repeat(Movie.Id, 1)).ToArray();
                    OnPropertyChanged(nameof(MovieIsAlreadyOnActiveList));
                }
                else
                    await _pageService.DisplayAlert("Error", "Could not add item to the active list. Try refreshing the lists first or use the TMDB webpage directly", "Ok");
            }
        }        

        private void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

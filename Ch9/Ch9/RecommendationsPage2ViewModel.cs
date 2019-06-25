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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Ch9
{
    public class RecommendationsPage2ViewModel : INotifyPropertyChanged
    {
        private readonly ISettings _settings;
        private readonly ITmdbCachedSearchClient _tmdbCachedSearchClient;
        private readonly ISearchResultFilter _searchResultFilter;
        private readonly IMovieDetailModelConfigurator _movieDetailModelConfigurator;
        private readonly IPageService _pageService;
        private readonly ICommand _updateMoviesListCommand;

        Task<GetMovieRecommendationsResult> _getMovieRecommendations;
        Task<GetSimilarMoviesResult> _getSimilarMovies;
        

        public MovieDetailModel Movie { get; private set; }

        private ObservableCollection<MovieDetailModel> _movies;

        public ObservableCollection<MovieDetailModel> Movies
        {
            get => _movies;
            set => SetProperty(ref _movies, value);
        }

        private bool _recommendationsOrSimilars;
        public bool RecommendationsOrSimilars
        {
            get => _recommendationsOrSimilars;
            set
            {
                if (SetProperty(ref _recommendationsOrSimilars, value))
                    _updateMoviesListCommand.Execute(null);
            }
                
                
        }

        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public RecommendationsPage2ViewModel(
            MovieDetailModel movie,
            ISettings settings,
            ITmdbCachedSearchClient tmdbCachedSearchClient,
            ISearchResultFilter searchResultFilter,
            IMovieDetailModelConfigurator movieDetailModelConfigurator,
            IPageService pageService)
        {
            _settings = settings;
            _tmdbCachedSearchClient = tmdbCachedSearchClient;
            _searchResultFilter = searchResultFilter;
            _movieDetailModelConfigurator = movieDetailModelConfigurator;
            _pageService = pageService;

            Movie = movie;
            IsRefreshing = false;

            _updateMoviesListCommand = new Command(async () => await UpdateMoviesList());

            AssignGetRecommendationsTasks();
        }

        private void AssignGetRecommendationsTasks()
        {
            _getMovieRecommendations = _tmdbCachedSearchClient.GetMovieRecommendations(Movie.Id, _settings.SearchLanguage, 1, 1000);
            _getSimilarMovies = _tmdbCachedSearchClient.GetSimilarMovies(Movie.Id, _settings.SearchLanguage, 1, 1000);
        }

        // Should be called by Page.OnAppearing()
        public async Task Initialize()
        {
            if (!(Movies?.Count > 0))
                await UpdateMoviesList();
        }

        public async Task UpdateMoviesList()
        {
            IsRefreshing = true;
            try
            {
                TmdbResponseBase result;

                if (RecommendationsOrSimilars)
                    result = await _getMovieRecommendations;
                else
                    result = await _getSimilarMovies;

                if (result.HttpStatusCode.IsSuccessCode())
                {
                    SearchResult deserializedApiResponse = JsonConvert.DeserializeObject<SearchResult>(result.Json);
                    List<MovieDetailModel> filteredResults = _searchResultFilter.FilterBySearchSettings(deserializedApiResponse.MovieDetailModels).ToList();

                    _movieDetailModelConfigurator.SetImageSrc(filteredResults);
                    _movieDetailModelConfigurator.SetGenreNamesFromGenreIds(filteredResults);
                    Utils.Utils.RefillList(Movies, filteredResults);
                }
                else
                    await _pageService.DisplayAlert("Network error", $"Application could not fetch the recommendation list. TMDB server responded with error code: {result.HttpStatusCode}", "Ok");
            }
            catch (Exception ex)
            {
                await _pageService.DisplayAlert("Exception", $"Exception occured, message: {ex.Message}", "Ok");                
            }
            IsRefreshing = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}

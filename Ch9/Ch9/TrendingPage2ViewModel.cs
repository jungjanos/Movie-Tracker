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
    // Maintains a collection for the actual trending movies
    // The bool switch controls whether the trending movies for the week (=true) or day (=false)
    // are displayed. 
    public class TrendingPage2ViewModel : INotifyPropertyChanged
    {
        private readonly ISettings _settings;
        private readonly ITmdbCachedSearchClient _tmdbCachedSearchClient;
        private readonly ISearchResultFilter _searchResultFilter;
        private readonly IMovieDetailModelConfigurator _movieDetailModelConfigurator;
        private readonly IPageService _pageService;

        Task<TrendingMoviesResult> _trendingThisWeekGetter;
        Task<TrendingMoviesResult> _trendingThisDayGetter;
        private readonly Command _updateTrendingMoviesListCommand;

        private bool _weekOrDaySwitch;

        // controls the setting of the Week/Day toggle switch on the UI
        // kicks of actions when toggles
        public bool WeekOrDaySwitch
        {
            get => _weekOrDaySwitch;
            set
            {
                if (SetProperty(ref _weekOrDaySwitch, value))
                    _updateTrendingMoviesListCommand.Execute(null);
            }

        }

        private ObservableCollection<MovieDetailModel> _trendingMovies;

        public ObservableCollection<MovieDetailModel> TrendingMovies
        {
            get => _trendingMovies;
            set => SetProperty(ref _trendingMovies, value);
        }

        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public ICommand RefreshCommand { get; private set; }
        public ICommand ItemTappedCommand { get; private set; }

        public TrendingPage2ViewModel(
            ISettings settings,
            ITmdbCachedSearchClient tmdbCachedSearchClient,
            ISearchResultFilter searchResultFilter,
            IMovieDetailModelConfigurator movieDetailModelConfigurator,
            IPageService pageService
            )
        {
            _settings = settings;
            _tmdbCachedSearchClient = tmdbCachedSearchClient;
            _searchResultFilter = searchResultFilter;
            _movieDetailModelConfigurator = movieDetailModelConfigurator;
            _pageService = pageService;
            _weekOrDaySwitch = true;
            _updateTrendingMoviesListCommand = new Command(async () => await UpdateTrendingMoviesList());
            RefreshCommand = new Command(async () => { RefreshTrendingGetters(); await UpdateTrendingMoviesList(); });
            ItemTappedCommand = new Command<MovieDetailModel>(async movie => await _pageService.PushAsync(movie));

            TrendingMovies = new ObservableCollection<MovieDetailModel>();

            RefreshTrendingGetters();
        }
        private void RefreshTrendingGetters()
        {
            _trendingThisWeekGetter = _tmdbCachedSearchClient.GetTrendingMovies(true, _settings.SearchLanguage, _settings.IncludeAdult, null, 1, 1000);
            _trendingThisDayGetter = _tmdbCachedSearchClient.GetTrendingMovies(false, _settings.SearchLanguage, _settings.IncludeAdult, null, 1, 1000);
        }

        // Should be called by Page.OnAppearing()
        public async Task Initialize()
        {
            if (!(TrendingMovies?.Count > 0))
                await UpdateTrendingMoviesList();
        }

        private async Task UpdateTrendingMoviesList()
        {
            IsRefreshing = true;
            try
            {
                TrendingMoviesResult result = await (WeekOrDaySwitch ? _trendingThisWeekGetter : _trendingThisDayGetter);

                if (result.HttpStatusCode.IsSuccessCode())
                {
                    SearchResult deserializedApiResponse = JsonConvert.DeserializeObject<SearchResult>(result.Json);
                    List<MovieDetailModel> filteredResults = _searchResultFilter.FilterBySearchSettings(deserializedApiResponse.MovieDetailModels).ToList();

                    _movieDetailModelConfigurator.SetImageSrc(filteredResults);
                    _movieDetailModelConfigurator.SetGenreNamesFromGenreIds(filteredResults);

                    TrendingMovies = new ObservableCollection<MovieDetailModel>(filteredResults);
                }
                else
                    await _pageService.DisplayAlert("Network error", $"Application could not fetch the Trendings list. TMDB server responded with error code: {result.HttpStatusCode}", "Ok");
            }
            catch (Exception ex)
            {
                await _pageService.DisplayAlert("Exception", $"Exception occured, message: {ex.Message}", "Ok");
                RefreshTrendingGetters();
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

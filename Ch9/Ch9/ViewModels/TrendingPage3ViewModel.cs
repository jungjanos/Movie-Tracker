using Ch9.ApiClient;
using Ch9.Models;
using Ch9.Services;
using Ch9.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Ch9.ViewModels
{
    // Tracks the trending movies for the current week and day
    public class TrendingPage3ViewModel : INotifyPropertyChanged
    {
        private readonly ISettings _settings;
        private readonly ITmdbCachedSearchClient _tmdbCachedSearchClient;
        private readonly ISearchResultFilter _resultFilter;
        private readonly IMovieDetailModelConfigurator _movieDetailConfigurator;
        private readonly IPageService _pageService;

        private SearchResult _trendingWeek;
        public SearchResult TrendingWeek
        {
            get => _trendingWeek;
            set => SetProperty(ref _trendingWeek, value);
        }

        private bool _isRefreshing = false;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        private SearchResult _trendingDay;
        public SearchResult TrendingDay
        {
            get => _trendingDay;
            set => SetProperty(ref _trendingDay, value);
        }

        // controls the setting of the Week/Day toggle switch on the UI        
        private bool _weekOrDaySwitch;
        public bool WeekOrDaySwitch
        {
            get => _weekOrDaySwitch;
            set => SetProperty(ref _weekOrDaySwitch, value);
        }

        public ICommand LoadNextTrendingWeekPageCommand { get; private set; }
        public ICommand LoadNextTrendingDayPageCommand { get; private set; }
        public ICommand RefreshTrendingWeekListCommand { get; private set; }
        public ICommand RefreshTrendingDayListCommand { get; private set; }
        public ICommand OnItemTappedCommand { get; private set; }


        public TrendingPage3ViewModel(ISettings settings,
            ITmdbCachedSearchClient tmdbCachedSearchClient,
            ISearchResultFilter resultFilter,
            IMovieDetailModelConfigurator movieDetailConfigurator,
            IPageService pageService)
        {
            _settings = settings;
            _tmdbCachedSearchClient = tmdbCachedSearchClient;
            _resultFilter = resultFilter;
            _movieDetailConfigurator = movieDetailConfigurator;
            _pageService = pageService;
            _weekOrDaySwitch = true;

            _trendingWeek = new SearchResult();
            _trendingWeek.InitializeOrClearMovieCollection();

            _trendingDay = new SearchResult();
            _trendingDay.InitializeOrClearMovieCollection();

            LoadNextTrendingWeekPageCommand = new Command(async () => await TryLoadingNextWeekPage());
            LoadNextTrendingDayPageCommand = new Command(async () => await TryLoadingNextDayPage());

            RefreshTrendingWeekListCommand = new Command(async () =>
            {
                ClearList(TrendingWeek);
                await TryLoadingNextWeekPage(1, 1000);
            });

            RefreshTrendingDayListCommand = new Command(async () =>
            {
                ClearList(TrendingDay);
                await TryLoadingNextDayPage(1, 1000);
            });

            OnItemTappedCommand = new Command<MovieDetailModel>(async movie => await _pageService.PushAsync(movie));
        }

        /// <summary>
        /// Must be called from View's OnAppearing() method.
        /// Ensure initial population of trending lists
        /// </summary>
        public async Task Initialize()
        {
            Task tw = Task.CompletedTask;
            Task td = Task.CompletedTask;

            if (TrendingWeek.Page == 0)
                tw = TryLoadingNextWeekPage(1, 1000);

            if (TrendingDay.Page == 0)
                td = TryLoadingNextDayPage(1, 1000);

            await Task.WhenAll(tw, td);
        }

        private void ClearList(SearchResult list)
        {
            list.MovieDetailModels.Clear();
            list.Page = 0;
            list.TotalPages = 0;
            list.TotalResults = 0;
        }

        public async Task RefreshTrendingWeekList(int retryCount = 0, int delayMilliseconds = 1000)
        {
            ClearList(TrendingWeek);
            await TryLoadingNextWeekPage(retryCount, delayMilliseconds);
        }

        public async Task TryLoadingNextWeekPage(int retryCount = 0, int delayMilliseconds = 1000)
        {
            if (TrendingWeek.Page == 0 || TrendingWeek.Page < TrendingWeek.TotalPages)
            {
                IsRefreshing = true;
                try
                {
                    var getNextPageResponse = await _tmdbCachedSearchClient.GetTrendingMovies(week: true, _settings.SearchLanguage, _settings.IncludeAdult, TrendingWeek.Page + 1, retryCount, delayMilliseconds);
                    if (!getNextPageResponse.HttpStatusCode.IsSuccessCode())
                    {
                        await _pageService.DisplayAlert("Error", $"Could not load the weekly trending list, service responded with: {getNextPageResponse.HttpStatusCode}", "Ok");
                        return;
                    }
                    SearchResult moviesOnNextPage = JsonConvert.DeserializeObject<SearchResult>(getNextPageResponse.Json);
                    moviesOnNextPage.MovieDetailModels = new ObservableCollection<MovieDetailModel>(_resultFilter.FilterBySearchSettings(moviesOnNextPage.MovieDetailModels));

                    Utils.Utils.AppendResult(TrendingWeek, moviesOnNextPage, _movieDetailConfigurator);
                }
                catch (Exception ex) { await _pageService.DisplayAlert("Error", $"Could not load the weekly trending list, service responded with: {ex.Message}", "Ok"); }
                finally { IsRefreshing = false; }
            }
        }
        public async Task RefreshTrendingDayList(int retryCount = 0, int delayMilliseconds = 1000)
        {
            ClearList(TrendingDay);
            await TryLoadingNextDayPage(retryCount, delayMilliseconds);
        }

        public async Task TryLoadingNextDayPage(int retryCount = 0, int delayMilliseconds = 1000)
        {
            if (TrendingDay.Page == 0 || TrendingDay.Page < TrendingDay.TotalPages)
            {
                IsRefreshing = true;
                try
                {
                    var getNextPageResponse = await _tmdbCachedSearchClient.GetTrendingMovies(week: false, _settings.SearchLanguage, _settings.IncludeAdult, TrendingDay.Page + 1, retryCount, delayMilliseconds);
                    if (!getNextPageResponse.HttpStatusCode.IsSuccessCode())
                    {
                        await _pageService.DisplayAlert("Error", $"Could not load the daily trending list, service responded with: {getNextPageResponse.HttpStatusCode}", "Ok");
                        return;
                    }
                    SearchResult moviesOnNextPage = JsonConvert.DeserializeObject<SearchResult>(getNextPageResponse.Json);
                    moviesOnNextPage.MovieDetailModels = new ObservableCollection<MovieDetailModel>(_resultFilter.FilterBySearchSettings(moviesOnNextPage.MovieDetailModels));

                    Utils.Utils.AppendResult(TrendingDay, moviesOnNextPage, _movieDetailConfigurator);
                }
                catch (Exception ex) { await _pageService.DisplayAlert("Error", $"Could not load the daily trending list, service responded with: {ex.Message}", "Ok"); }
                finally { IsRefreshing = false; }
            }
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

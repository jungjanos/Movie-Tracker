using Ch9.Services;
using Ch9.Ui.Contracts.Models;
//using Ch9.Utils;
using Ch9.Services.Contracts;

using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Ch9.Infrastructure.Extensions;
using Ch9.Services.MovieListServices;

namespace Ch9.ViewModels
{
    // Tracks the trending movies for the current week and day
    public class TrendingPage3ViewModel : ViewModelBase
    {
        private readonly ISettings _settings;        
        private readonly ITmdbApiService _tmdbApiService;
        private readonly Utils.ISearchResultFilter _resultFilter;
        private readonly IMovieDetailModelConfigurator _movieDetailConfigurator;

        private SearchResult _trendingWeek;
        public SearchResult TrendingWeek
        {
            get => _trendingWeek;
            set => SetProperty(ref _trendingWeek, value);
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
            ITmdbApiService tmdbApiService,
            Utils.ISearchResultFilter resultFilter,
            IMovieDetailModelConfigurator movieDetailConfigurator,
            IPageService pageService) : base(pageService)
        {
            _settings = settings;            
            _tmdbApiService = tmdbApiService;
            _resultFilter = resultFilter;
            _movieDetailConfigurator = movieDetailConfigurator;
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

            /// Ensures initial population of trending lists
            Func<Task> initializationAction = async () =>
            {
                Task tw = Task.CompletedTask;
                Task td = Task.CompletedTask;

                if (TrendingWeek.Page == 0)
                    tw = TryLoadingNextWeekPage(1, 1000);

                if (TrendingDay.Page == 0)
                    td = TryLoadingNextDayPage(1, 1000);

                await Task.WhenAll(tw, td);
            };

            ConfigureInitialization(initializationAction, false);
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
                IsBusy = true;
                try
                {
                    var nextPageResponse = await _tmdbApiService.TryGetTrendingMovies(week: true, _settings.SearchLanguage, !_settings.SafeSearch, TrendingWeek.Page + 1, retryCount, delayMilliseconds);
                    if (nextPageResponse.HttpStatusCode.IsSuccessCode())
                    {
                        var moviesOnNextPage = nextPageResponse.TrendingMovies;
                        moviesOnNextPage.MovieDetailModels = new ObservableCollection<MovieDetailModel>(_resultFilter.FilterBySearchSettings(moviesOnNextPage.MovieDetailModels));

                        TrendingWeek.AppendResult(moviesOnNextPage, _movieDetailConfigurator);
                    }
                    else
                        await _pageService.DisplayAlert("Error", $"Could not load the weekly trending list, service responded with: {nextPageResponse.HttpStatusCode}", "Ok");                     
                }
                catch (Exception ex) { await _pageService.DisplayAlert("Error", $"Could not load the weekly trending list, service responded with: {ex.Message}", "Ok"); }
                finally { IsBusy = false; }
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
                IsBusy = true;
                try
                {
                    var nextPageResponse = await _tmdbApiService.TryGetTrendingMovies(week: false, _settings.SearchLanguage, !_settings.SafeSearch, TrendingDay.Page + 1, retryCount, delayMilliseconds);
                    
                    if (nextPageResponse.HttpStatusCode.IsSuccessCode())
                    {
                        var moviesOnNextPage = nextPageResponse.TrendingMovies;
                        moviesOnNextPage.MovieDetailModels = new ObservableCollection<MovieDetailModel>(_resultFilter.FilterBySearchSettings(moviesOnNextPage.MovieDetailModels));
                        
                        TrendingDay.AppendResult(moviesOnNextPage, _movieDetailConfigurator); 
                    }
                    else
                        await _pageService.DisplayAlert("Error", $"Could not load the daily trending list, service responded with: {nextPageResponse.HttpStatusCode}", "Ok");
                }
                catch (Exception ex) { await _pageService.DisplayAlert("Error", $"Could not load the daily trending list, service responded with: {ex.Message}", "Ok"); }
                finally { IsBusy = false; }
            }
        }
    }
}

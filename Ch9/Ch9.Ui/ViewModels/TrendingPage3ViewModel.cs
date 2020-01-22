using Ch9.Models;
using Ch9.Services;
using Ch9.Services.Contracts;
using Ch9.Services.MovieListServices;

using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Ch9.ViewModels
{
    // Tracks the trending movies for the current week and day
    public class TrendingPage3ViewModel : ViewModelBase
    {
        private readonly ITrendingMoviesService _trendingMoviesService;
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

        public TrendingPage3ViewModel(ITrendingMoviesService trendingMoviesService, IPageService pageService) : base(pageService)
        {
            _trendingMoviesService = trendingMoviesService;
            _weekOrDaySwitch = true;

            _trendingWeek = new SearchResult();
            _trendingWeek.InitializeOrClearMovieCollection();

            _trendingDay = new SearchResult();
            _trendingDay.InitializeOrClearMovieCollection();

            LoadNextTrendingWeekPageCommand = new Command(async () => await TryLoadingNextPage(week: true));
            LoadNextTrendingDayPageCommand = new Command(async () => await TryLoadingNextPage(week: false));

            RefreshTrendingWeekListCommand = new Command(async () =>
            {
                TrendingWeek.InitializeOrClearMovieCollection();
                await TryLoadingNextPage(week: true, 1, 1000);
            });

            RefreshTrendingDayListCommand = new Command(async () =>
            {
                TrendingDay.InitializeOrClearMovieCollection();
                await TryLoadingNextPage(week: false, 1, 1000);
            });

            OnItemTappedCommand = new Command<MovieDetailModel>(async movie => await _pageService.PushAsync(movie));

            /// Ensures initial population of trending lists by using concurrency
            Func<Task> initializationAction = async () =>
            {
                Task tw = Task.CompletedTask;
                Task td = Task.CompletedTask;

                if (TrendingWeek.Page == 0)
                    tw = TryLoadingNextPage(week: true, 1, 1000);

                if (TrendingDay.Page == 0)
                    td = TryLoadingNextPage(week: false, 1, 1000);

                await Task.WhenAll(tw, td);
            };

            ConfigureInitialization(initializationAction, false);
        }


        // Tries to load the next page of the currently selected trending list
        private async Task TryLoadingNextPage(bool week, int retryCount = 0, int delayMilliseconds = 1000)
        {
            var targetList = week ? TrendingWeek : TrendingDay;

            if (targetList.Page == 0 || targetList.Page < targetList.TotalPages)
            {
                IsBusy = true;

                try
                {
                    var page = await _trendingMoviesService.LoadTrendingPage(week: week, page: targetList.Page + 1, retryCount, delayMilliseconds);
                    targetList.AppendResult(page);
                }
                catch (Exception ex) { await _pageService.DisplayAlert("Error", ex.Message, "Ok"); }
                finally { IsBusy = false; }
            }
        }
    }
}

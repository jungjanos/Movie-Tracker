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
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Ch9.ViewModels
{
    public class RecommendationsPage3ViewModel : INotifyPropertyChanged
    {
        private readonly ISettings _settings;
        private readonly ITmdbCachedSearchClient _tmdbCachedSearchClient;
        private readonly ISearchResultFilter _searchResultFilter;
        private readonly IMovieDetailModelConfigurator _movieDetailModelConfigurator;
        private readonly IPageService _pageService;

        public MovieDetailModel Movie { get; private set; }

        private SearchResult _recommendedMovies;
        public SearchResult RecommendedMovies
        {
            get => _recommendedMovies;
            set => SetProperty(ref _recommendedMovies, value);
        }

        private SearchResult _similarMovies;
        public SearchResult SimilarMovies
        {
            get => _similarMovies;
            set => SetProperty(ref _similarMovies, value);
        }


        private bool _recommendationsOrSimilars;
        public bool RecommendationsOrSimilars
        {
            get => _recommendationsOrSimilars;
            set => SetProperty(ref _recommendationsOrSimilars, value);
        }

        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }
        public ICommand LoadNextRecommendedMoviesPageCommand { get; private set; }
        public ICommand LoadNextSimilarMoviesPageCommand { get; private set; }
        public ICommand RefreshRecommendedMoviesCommand { get; private set; }
        public ICommand RefreshSimilarMoviesCommand { get; private set; }
        public ICommand OnItemTappedCommand { get; private set; }

        public RecommendationsPage3ViewModel(MovieDetailModel movie,
            ISettings settings,
            ITmdbCachedSearchClient tmdbCachedSearchClient,
            ISearchResultFilter searchResultFilter,
            IMovieDetailModelConfigurator movieDetailModelConfigurator,
            IPageService pageService)
        {
            Movie = movie;

            _settings = settings;
            _tmdbCachedSearchClient = tmdbCachedSearchClient;
            _searchResultFilter = searchResultFilter;
            _movieDetailModelConfigurator = movieDetailModelConfigurator;
            _pageService = pageService;
            _recommendationsOrSimilars = true;

            _recommendedMovies = new SearchResult
            {
                MovieDetailModels = new ObservableCollection<MovieDetailModel>(),
                Page = 0,
                TotalPages = 0,
                TotalResults = 0
            };

            _similarMovies = new SearchResult
            {
                MovieDetailModels = new ObservableCollection<MovieDetailModel>(),
                Page = 0,
                TotalPages = 0,
                TotalResults = 0
            };

            LoadNextRecommendedMoviesPageCommand = new Command(async () => await TryLoadingNextRecommendedMoviesPage());
            LoadNextSimilarMoviesPageCommand = new Command(async () => await TryLoadingNextSimilarMoviesPage());

            RefreshRecommendedMoviesCommand = new Command(async () =>
            {
                ClearList(RecommendedMovies);
                await TryLoadingNextRecommendedMoviesPage(1, 1000);
            });

            RefreshSimilarMoviesCommand = new Command(async () =>
            {
                ClearList(SimilarMovies);
                await TryLoadingNextSimilarMoviesPage(1, 1000);
            });

            OnItemTappedCommand = new Command<MovieDetailModel>(async mov => await _pageService.PushAsync(mov));
        }

        /// <summary>
        /// Must be called from View's OnAppearing() method.
        /// Ensure initial population of recommended and similars lists
        /// </summary>
        public async Task Initialize()
        {
            Task tr = Task.CompletedTask;
            Task ts = Task.CompletedTask;

            if (RecommendedMovies.Page == 0)
                tr = TryLoadingNextRecommendedMoviesPage(1, 1000);

            if (SimilarMovies.Page == 0)
                ts = TryLoadingNextSimilarMoviesPage(1, 1000);

            await Task.WhenAll(tr, ts);
        }

        private void ClearList(SearchResult list)
        {
            list.MovieDetailModels.Clear();
            list.Page = 0;
            list.TotalPages = 0;
            list.TotalResults = 0;
        }

        public async Task RefreshRecommendedMovies(int retryCount = 0, int delayMilliseconds = 1000)
        {
            ClearList(RecommendedMovies);
            await TryLoadingNextRecommendedMoviesPage(retryCount, delayMilliseconds);
        }

        public async Task TryLoadingNextRecommendedMoviesPage(int retryCount = 0, int delayMilliseconds = 1000)
        {
            if (RecommendedMovies.Page == 0 || RecommendedMovies.Page < RecommendedMovies.TotalPages)
            {
                IsRefreshing = true;
                try
                {
                    var getNextPageResponse = await _tmdbCachedSearchClient.GetMovieRecommendations(Movie.Id, _settings.SearchLanguage, RecommendedMovies.Page+1, retryCount, delayMilliseconds);
                    if (!getNextPageResponse.HttpStatusCode.IsSuccessCode())
                    {
                        await _pageService.DisplayAlert("Error", $"Could not update the recommended movies list, service responded with: {getNextPageResponse.HttpStatusCode}", "Ok");
                        return;
                    }
                    SearchResult moviesOnNextPage = JsonConvert.DeserializeObject<SearchResult>(getNextPageResponse.Json);
                    moviesOnNextPage.MovieDetailModels = new ObservableCollection<MovieDetailModel>(_searchResultFilter.FilterBySearchSettings(moviesOnNextPage.MovieDetailModels));

                    Utils.Utils.AppendResult(RecommendedMovies, moviesOnNextPage, _movieDetailModelConfigurator);
                } catch(Exception ex){ await _pageService.DisplayAlert("Error", $"Could not update the recommended movies list, service responded with: {ex.Message}", "Ok"); }
                finally { IsRefreshing = false; }
            }
        }

        public async Task RefreshSimilarMovies(int retryCount = 0, int delayMilliseconds = 1000)
        {
            ClearList(SimilarMovies);
            await TryLoadingNextSimilarMoviesPage(retryCount, delayMilliseconds);
        }

        public async Task TryLoadingNextSimilarMoviesPage(int retryCount = 0, int delayMilliseconds = 1000)
        {
            if (SimilarMovies.Page == 0 || SimilarMovies.Page < SimilarMovies.TotalPages)
            {
                IsRefreshing = true;
                try
                {
                    var getNextPageResponse = await _tmdbCachedSearchClient.GetSimilarMovies(Movie.Id, _settings.SearchLanguage, SimilarMovies.Page + 1, retryCount, delayMilliseconds);
                    if (!getNextPageResponse.HttpStatusCode.IsSuccessCode())
                    {
                        await _pageService.DisplayAlert("Error", $"Could not update the similar movies list, service responded with: {getNextPageResponse.HttpStatusCode}", "Ok");
                        return;
                    }
                    SearchResult moviesOnNextPage = JsonConvert.DeserializeObject<SearchResult>(getNextPageResponse.Json);
                    moviesOnNextPage.MovieDetailModels = new ObservableCollection<MovieDetailModel>(_searchResultFilter.FilterBySearchSettings(moviesOnNextPage.MovieDetailModels));

                    Utils.Utils.AppendResult(SimilarMovies, moviesOnNextPage, _movieDetailModelConfigurator);
                }
                catch (Exception ex) { await _pageService.DisplayAlert("Error", $"Could not update the similar movies list, service responded with: {ex.Message}", "Ok"); }
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

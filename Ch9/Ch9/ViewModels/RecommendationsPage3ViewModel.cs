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
    public class RecommendationsPage3ViewModel : ViewModelBase
    {
        private readonly ISettings _settings;        
        private readonly ITmdbApiService _tmdbApiService;
        private readonly Utils.ISearchResultFilter _searchResultFilter;
        private readonly IMovieDetailModelConfigurator _movieDetailModelConfigurator;

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

        public ICommand LoadNextRecommendedMoviesPageCommand { get; private set; }
        public ICommand LoadNextSimilarMoviesPageCommand { get; private set; }
        public ICommand RefreshRecommendedMoviesCommand { get; private set; }
        public ICommand RefreshSimilarMoviesCommand { get; private set; }
        public ICommand OnItemTappedCommand { get; private set; }

        public RecommendationsPage3ViewModel(MovieDetailModel movie,
            ISettings settings,            
            ITmdbApiService tmdbApiService,
            Utils.ISearchResultFilter searchResultFilter,
            IMovieDetailModelConfigurator movieDetailModelConfigurator,
            IPageService pageService) : base(pageService)
        {
            Movie = movie;

            _settings = settings;
            _tmdbApiService = tmdbApiService;
            _searchResultFilter = searchResultFilter;
            _movieDetailModelConfigurator = movieDetailModelConfigurator;
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

            // Ensures that both the recommendations and the similars lists are populated
            Func<Task> initializationAction = async () =>
            {
                Task tr = Task.CompletedTask;
                Task ts = Task.CompletedTask;

                if (RecommendedMovies.Page == 0)
                    tr = TryLoadingNextRecommendedMoviesPage(1, 1000);

                if (SimilarMovies.Page == 0)
                    ts = TryLoadingNextSimilarMoviesPage(1, 1000);

                await Task.WhenAll(tr, ts);
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

        public async Task RefreshRecommendedMovies(int retryCount = 0, int delayMilliseconds = 1000)
        {
            ClearList(RecommendedMovies);
            await TryLoadingNextRecommendedMoviesPage(retryCount, delayMilliseconds);
        }

        public async Task TryLoadingNextRecommendedMoviesPage(int retryCount = 0, int delayMilliseconds = 1000)
        {
            if (RecommendedMovies.Page == 0 || RecommendedMovies.Page < RecommendedMovies.TotalPages)
            {
                IsBusy = true;
                try
                {
                    var response = await _tmdbApiService.TryGetMovieRecommendations(Movie.Id, _settings.SearchLanguage, RecommendedMovies.Page + 1, retryCount, delayMilliseconds);
                    if (response.HttpStatusCode.IsSuccessCode())
                    {
                        var moviesOnNextPage = response.MovieRecommendations;
                        moviesOnNextPage.MovieDetailModels = new ObservableCollection<MovieDetailModel>(_searchResultFilter.FilterBySearchSettings(moviesOnNextPage.MovieDetailModels));
                        RecommendedMovies.AppendResult(moviesOnNextPage, _movieDetailModelConfigurator);
                    }
                    else
                        await _pageService.DisplayAlert("Error", $"Could not update the recommended movies list, service responded with: {response.HttpStatusCode}", "Ok");
                }
                catch (Exception ex) { await _pageService.DisplayAlert("Error", $"Could not update the recommended movies list, service responded with: {ex.Message}", "Ok"); }
                finally { IsBusy = false; }
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
                IsBusy = true;
                try
                {
                    var response = await _tmdbApiService.TryGetSimilarMovies(Movie.Id, _settings.SearchLanguage, SimilarMovies.Page + 1, retryCount, delayMilliseconds);

                    if (response.HttpStatusCode.IsSuccessCode())
                    {
                        var moviesOnNextPage = response.SimilarMovies;
                        moviesOnNextPage.MovieDetailModels = new ObservableCollection<MovieDetailModel>(_searchResultFilter.FilterBySearchSettings(moviesOnNextPage.MovieDetailModels));
                        SimilarMovies.AppendResult(moviesOnNextPage, _movieDetailModelConfigurator);
                    }
                    else
                        await _pageService.DisplayAlert("Error", $"Could not update the similar movies list, service responded with: {response.HttpStatusCode}", "Ok");
                }
                catch (Exception ex) { await _pageService.DisplayAlert("Error", $"Could not update the similar movies list, service responded with: {ex.Message}", "Ok"); }
                finally { IsBusy = false; }
            }
        }
    }
}

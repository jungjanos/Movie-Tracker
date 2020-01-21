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
    public class RecommendationsPage3ViewModel : ViewModelBase
    {
        private readonly IMovieRecommendationService _movieRecommendationService;

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

        public RecommendationsPage3ViewModel(MovieDetailModel movie, IMovieRecommendationService movieRecommendationService, IPageService pageService)
            : base(pageService)
        {
            Movie = movie;

            _movieRecommendationService = movieRecommendationService;
            _recommendationsOrSimilars = true;

            _recommendedMovies = new SearchResult();
            _recommendedMovies.InitializeOrClearMovieCollection();

            _similarMovies = new SearchResult();
            _similarMovies.InitializeOrClearMovieCollection();

            LoadNextRecommendedMoviesPageCommand = new Command(async () => await TryLoadingNextRecommendedMoviesPage());
            LoadNextSimilarMoviesPageCommand = new Command(async () => await TryLoadingNextSimilarMoviesPage());

            RefreshRecommendedMoviesCommand = new Command(async () =>
            {
                RecommendedMovies.InitializeOrClearMovieCollection();
                await TryLoadingNextRecommendedMoviesPage(1, 1000);
            });

            RefreshSimilarMoviesCommand = new Command(async () =>
            {
                SimilarMovies.InitializeOrClearMovieCollection();
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

        private async Task TryLoadingNextRecommendedMoviesPage(int retryCount = 0, int delayMilliseconds = 1000)
        {
            if (RecommendedMovies.Page == 0 || RecommendedMovies.Page < RecommendedMovies.TotalPages)
            {
                IsBusy = true;
                try
                {
                    var page = await _movieRecommendationService.LoadMovieRecommendationsPage(Movie.Id, RecommendedMovies.Page + 1, retryCount, delayMilliseconds);
                    RecommendedMovies.AppendResult(page);
                }
                catch (Exception ex) { await _pageService.DisplayAlert("Error", ex.Message, "Ok"); }
                finally { IsBusy = false; }
            }
        }

        private async Task TryLoadingNextSimilarMoviesPage(int retryCount = 0, int delayMilliseconds = 1000)
        {
            if (SimilarMovies.Page == 0 || SimilarMovies.Page < SimilarMovies.TotalPages)
            {
                IsBusy = true;
                try
                {
                    var page = await _movieRecommendationService.LoadSimilarMoviesPage(Movie.Id, SimilarMovies.Page + 1, retryCount, delayMilliseconds);
                    SimilarMovies.AppendResult(page);
                }
                catch (Exception ex) { await _pageService.DisplayAlert("Error", ex.Message, "Ok"); }
                finally { IsBusy = false; }
            }
        }
    }
}

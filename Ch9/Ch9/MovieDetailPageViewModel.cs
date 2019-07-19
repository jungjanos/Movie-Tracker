using Ch9.ApiClient;
using Ch9.Models;
using Ch9.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private readonly UsersMovieListsService2 _movieListsService2;
        private readonly IMovieDetailModelConfigurator _movieDetailModelConfigurator;
        private readonly IPageService _pageService;
        private readonly Task _fetchGallery;
        private readonly ReviewsPageViewModel _reviewsPageViewModel;

        public bool? MovieIsAlreadyOnActiveList => _movieListsService2.CustomListsService.CheckIfMovieIsOnActiveList(Movie.Id);
        private bool _movieHasReviews;
        public bool MovieHasReviews
        {
            get => _movieHasReviews;
            set => SetProperty(ref _movieHasReviews, value);
        }

        private bool _movieStatesFetchFinished;
        public bool MovieStatesFetchFinished
        {
            get => _movieStatesFetchFinished;
            set => SetProperty(ref _movieStatesFetchFinished, value);
        }

        private AccountMovieStates _movieStates;
        public AccountMovieStates MovieStates
        {
            get => _movieStates;
            set => SetProperty(ref _movieStates, value);
        }
        
        public ICommand HomeCommand { get; private set; }
        public ICommand RecommendationsCommand { get; private set; }
        public ICommand ReviewsCommand { get; private set; }
        public ICommand ToggleWatchlistCommand { get; private set; }
        public ICommand AddToListCommand { get; private set; }        
        public ICommand ToggleFavoriteCommand { get; private set; }

        public MovieDetailPageViewModel(
            MovieDetailModel movie,
            ISettings settings,
            ITmdbCachedSearchClient cachedSearchClient,
            UsersMovieListsService2 movieListsService2,
            IMovieDetailModelConfigurator movieDetailModelConfigurator,
            IPageService pageService)
        {
            Movie = movie;
            _settings = settings;
            _cachedSearchClient = cachedSearchClient;
            _movieListsService2 = movieListsService2;
            _movieDetailModelConfigurator = movieDetailModelConfigurator;
            _pageService = pageService;
            _fetchGallery = UpdateImageDetailCollection();
            MovieHasReviews = false;
            MovieStatesFetchFinished = false;
            _reviewsPageViewModel = new ReviewsPageViewModel(this, _cachedSearchClient);            
            
            HomeCommand = new Command(async () => await _pageService.PopToRootAsync());
            ReviewsCommand = new Command(async () => await _pageService.PushAsync(_reviewsPageViewModel), () => MovieStatesFetchFinished);
            RecommendationsCommand = new Command(async () => await _pageService.PushRecommendationsPageAsync(Movie));
            ToggleWatchlistCommand = new Command(async () => await OnToggleWatchlistCommand());
            AddToListCommand = new Command(async () => await OnAddToListCommand());
            ToggleFavoriteCommand = new Command(async () => await OnToggleFavoriteCommand(), () => MovieStatesFetchFinished);
        }

        public async Task Initialize()
        {
            _ = FetchMovieStates();
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
            }, TaskScheduler.FromCurrentSynchronizationContext());
            return galleryCollectionReady;
        }      
        
        public async Task OnToggleWatchlistCommand()
        {
            if (!_settings.HasTmdbAccount)
            {
                await _pageService.DisplayAlert("Info", "You have to log in with a user account to use this function", "Ok");
                return;
            }

            bool desiredState = !MovieStates.OnWatchlist;

            try
            {
                await _movieListsService2.WatchlistService.ToggleWatchlistState(Movie, desiredState);
                MovieStates.OnWatchlist = desiredState;
                OnPropertyChanged(nameof(MovieStates));
            }
            catch (Exception ex)
            { await _pageService.DisplayAlert("Error", $"Could not change watchlist state, service responded with: {ex.Message}", "Ok"); }
        }

        public async Task OnAddToListCommand()
        {
            if (!_settings.HasTmdbAccount)
            {
                await _pageService.DisplayAlert("Info", "You have to log in with a user account to use this function", "Ok");
                return;
            }

            if (MovieIsAlreadyOnActiveList == null)
            {
                await _pageService.DisplayAlert("Info", "You have to select a valid public list to be able to add movies to it", "Cancel");
                return;
            }

            try
            {
                if (MovieIsAlreadyOnActiveList == true)
                {
                    await _movieListsService2.CustomListsService.RemoveMovieFromActiveList(Movie.Id);
                    OnPropertyChanged(nameof(MovieIsAlreadyOnActiveList));
                    return;
                }

                if (MovieIsAlreadyOnActiveList == false)
                {
                    await _movieListsService2.CustomListsService.AddMovieToActiveList(Movie);
                    OnPropertyChanged(nameof(MovieIsAlreadyOnActiveList));
                }
            }
            catch (Exception ex) { await _pageService.DisplayAlert("Error",$"Service responded with: {ex.Message}","Ok"); }            
        }

        public async Task OnToggleFavoriteCommand()
        {
            if (!_settings.HasTmdbAccount)
            {
                await _pageService.DisplayAlert("Info", "You have to log in with a user account to use this function", "Ok");
                return;
            }

            bool desiredState = !MovieStates.IsFavorite;

            try
            {
                await _movieListsService2.FavoriteMoviesListService.ToggleFavoriteState(Movie, desiredState);
                MovieStates.IsFavorite = desiredState;
                OnPropertyChanged(nameof(MovieStates));
            } catch (Exception ex)
            { await _pageService.DisplayAlert("Error", $"Could not change favorite state, service responded with: {ex.Message}", "Ok");}
        }
        
        // TODO : property should set be with awaiter from UI thread!!!
        private async Task FetchMovieStates()
        {
            GetAccountMovieStatesResult response = await _cachedSearchClient.GetAccountMovieStates(Movie.Id, guestSessionId: null, retryCount: 3, delayMilliseconds: 1000);
            if (response.HttpStatusCode.IsSuccessCode())
            {
                MovieStates = response.States;
                MovieStatesFetchFinished = true;
                RefreshCanExecutes();
            }
        }

        private void RefreshCanExecutes()
        {
            ((Command)ReviewsCommand).ChangeCanExecute();
            ((Command)ToggleFavoriteCommand).ChangeCanExecute();
        }

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

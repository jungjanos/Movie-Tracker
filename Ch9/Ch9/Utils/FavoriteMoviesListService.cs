using Ch9.ApiClient;
using Ch9.Models;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Ch9.Utils
{
    public class FavoriteMoviesListService : INotifyPropertyChanged
    {
        private readonly ISettings _settings;
        private readonly ITmdbCachedSearchClient _tmdbCachedSearchClient;
        private readonly IMovieDetailModelConfigurator _movieDetailConfigurator;
        private readonly ICommand _sortOptionChangedCommand;

        private SearchResult _favoriteMovies;
        public SearchResult FavoriteMovies
        {
            get => _favoriteMovies;
            set => SetProperty(ref _favoriteMovies, value);
        }

        private string _sortBy;
        public string SortBy
        {
            get => _sortBy;
            set
            {
                if (SetProperty(ref _sortBy, value))
                    _sortOptionChangedCommand.Execute(null);
            } 
        }

        public FavoriteMoviesListService(
            ISettings settings,
            ITmdbCachedSearchClient tmdbCachedSearchClient,
            IMovieDetailModelConfigurator movieDetailConfigurator)
        {
            _settings = settings;
            _tmdbCachedSearchClient = tmdbCachedSearchClient;
            _movieDetailConfigurator = movieDetailConfigurator;

            _sortBy = "created_at.desc";

            _favoriteMovies = new SearchResult
            {
                MovieDetailModels = new ObservableCollection<MovieDetailModel>(),
                Page = 0,
                TotalPages = 0,
                TotalResults = 0
            };

            _sortOptionChangedCommand = new Command(async () => await RefreshFavoriteMoviesList(1, 1000));
        }

        private void ClearFavoriteList()
        {
            FavoriteMovies.MovieDetailModels.Clear();
            FavoriteMovies.Page = 0;
            FavoriteMovies.TotalPages = 0;
            FavoriteMovies.TotalResults = 0;
        }

        /// <summary>
        /// Can throw. 
        /// First resets the client side collection of the favorite list to its initial state.
        /// Tries to reload the first page of the favorite list from the server.
        /// The Webrequest respects the ascending/descending property for the search. 
        /// </summary>        
        public async Task RefreshFavoriteMoviesList(int retryCount = 0, int delayMilliseconds = 1000)
        {
            ClearFavoriteList();

            if (!_settings.HasTmdbAccount)
                throw new Exception("Account error: user is not signed in");

            GetFavoriteMoviesResult getFavoriteList = await _tmdbCachedSearchClient.GetFavoriteMovies(language: _settings.SearchLanguage, sortBy: SortBy, page: 1, retryCount: retryCount, delayMilliseconds: delayMilliseconds);
            if (!getFavoriteList.HttpStatusCode.IsSuccessCode())
                throw new Exception($"Could not refresh favorite list, TMDB server responded with {getFavoriteList.HttpStatusCode}");

            SearchResult moviesOnFavoriteList = JsonConvert.DeserializeObject<SearchResult>(getFavoriteList.Json);

            Utils.AppendResult(FavoriteMovies, moviesOnFavoriteList, _movieDetailConfigurator);
        }

        public async Task TryLoadNextPage(int retryCount = 0, int delayMilliseconds = 1000)
        {
            if (!_settings.HasTmdbAccount)
                throw new Exception("Account error: user is not signed in");

            if (!CanLoad)
                return;

            GetFavoriteMoviesResult getFavoriteList = await _tmdbCachedSearchClient.GetFavoriteMovies(language: _settings.SearchLanguage, sortBy: SortBy, page: FavoriteMovies.Page + 1, retryCount: retryCount, delayMilliseconds: delayMilliseconds);

            if (!getFavoriteList.HttpStatusCode.IsSuccessCode())
                throw new Exception($"Could not load favorite list, TMDB server responded with {getFavoriteList.HttpStatusCode}");

            SearchResult moviesOnFavoriteListPage = JsonConvert.DeserializeObject<SearchResult>(getFavoriteList.Json);

            Utils.AppendResult(FavoriteMovies, moviesOnFavoriteListPage, _movieDetailConfigurator);
            OnPropertyChanged(nameof(CanLoad));
        }

        public bool CanLoad => FavoriteMovies.Page == 0 ? false : (FavoriteMovies.Page >= FavoriteMovies.TotalPages ? false : true);

        /// <summary>
        /// Can throw. 
        /// Tries to add or remove a movie to/from the favorites depending on the desired state. 
        /// The current favorite status of the movie both on the local list and on the server is not checked. 
        /// Trying to add (or remove) a movie which is (or is not) on the local or server's favorite list does not throw. 
        /// </summary>        
        /// <param name="desiredState">true: tries to add, false: tries to remove</param>        
        public async Task ToggleFavoriteState(MovieDetailModel movie, bool desiredState, int retryCount = 1, int delayMilliseconds = 1000)
        {
            if (!_settings.HasTmdbAccount)
                throw new Exception("Account error: user is not signed in");

            if (movie == null)
                throw new Exception("Movie is invalid");

            UpdateFavoriteListResult response = await _tmdbCachedSearchClient.UpdateFavoriteList("movie", desiredState, movie.Id, retryCount, delayMilliseconds);

            if (response.HttpStatusCode.IsSuccessCode())
            {
                if (desiredState) // we added the movie to the server's favorite list
                {
                    if (!FavoriteMovies.MovieDetailModels.Select(movie_ => movie_.Id).Contains(movie.Id))
                        FavoriteMovies.MovieDetailModels.Add(movie);
                }
                else // we removed the movie from the server's favorite list
                {
                    var movieToRemove = FavoriteMovies.MovieDetailModels.FirstOrDefault(movie_ => movie_.Id == movie.Id);
                    FavoriteMovies.MovieDetailModels.Remove(movieToRemove);
                }
            }
            else
                throw new Exception($"Could not update favorite state, server responded with: {response.HttpStatusCode}");
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


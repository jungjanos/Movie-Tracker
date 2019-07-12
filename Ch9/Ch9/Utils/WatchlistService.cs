using Ch9.ApiClient;
using Ch9.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Ch9.Utils
{
    public class WatchlistService : INotifyPropertyChanged
    {
        private readonly ISettings _settings;
        private readonly ITmdbCachedSearchClient _tmdbCachedSearchClient;
        private readonly IMovieDetailModelConfigurator _movieDetailConfigurator;

        private SearchResult _watchlist;
        public SearchResult Watchlist
        {
            get => _watchlist;
            set => SetProperty(ref _watchlist, value);
        }

        public string SortBy;

        public WatchlistService(ISettings settings,
            ITmdbCachedSearchClient tmdbCachedSearchClient,
            IMovieDetailModelConfigurator movieDetailConfigurator)
        {
            _settings = settings;
            _tmdbCachedSearchClient = tmdbCachedSearchClient;
            _movieDetailConfigurator = movieDetailConfigurator;

            SortBy = "created_at.desc";

            _watchlist = new SearchResult
            {
                MovieDetailModels = new ObservableCollection<MovieDetailModel>(),
                Page = 0,
                TotalPages = 0,
                TotalResults = 0
            };
        }

        private void ClearWatchlist()
        {
            Watchlist.MovieDetailModels.Clear();
            Watchlist.Page = 0;
            Watchlist.TotalPages = 0;
            Watchlist.TotalResults = 0;
        }

        /// <summary>
        /// Can throw. 
        /// First resets the client side collection of the watchlist to its initial state.
        /// Tries to reload the first page of the watchlist from the server.
        /// The Webrequest respects the ascending/descending property for the search. 
        /// </summary>        
        public async Task RefreshWatchlist(int retryCount = 0, int delayMilliseconds = 1000)
        {
            ClearWatchlist();

            if (!_settings.HasTmdbAccount)
                throw new Exception("Account error: user is not signed in");

            GetMovieWatchlistResult getWatchlist = await _tmdbCachedSearchClient.GetMovieWatchlist(language: _settings.SearchLanguage, sortBy: SortBy, page: 1, retryCount: retryCount, delayMilliseconds: delayMilliseconds);
            if (!getWatchlist.HttpStatusCode.IsSuccessCode())
                throw new Exception($"Could not refresh watchlist, TMDB server responded with {getWatchlist.HttpStatusCode}");

            SearchResult moviesOnWatchlist = JsonConvert.DeserializeObject<SearchResult>(getWatchlist.Json);

            Utils.AppendResult(Watchlist, moviesOnWatchlist, _movieDetailConfigurator);
        }

        /// <summary>
        /// Can throw. 
        /// Tries to add or remove a movie to/from the watchlist depending on the desired state. 
        /// The current watchlist status of the movie both on the local list and on the server is not checked. 
        /// Trying to add (or remove) a movie which is (or is not) on the local or server's watchlist list does not throw. 
        /// </summary>        
        /// <param name="desiredState">true: tries to add, false: tries to remove</param>        
        public async Task ToggleWatchlistState(MovieDetailModel movie, bool desiredState, int retryCount = 1, int delayMilliseconds = 1000)
        {
            if (!_settings.HasTmdbAccount)
                throw new Exception("Account error: user is not signed in");

            if (movie == null)
                throw new Exception("Movie is invalid");

            UpdateWatchlistResult response = await _tmdbCachedSearchClient.UpdateWatchlist("movie", desiredState, movie.Id, retryCount, delayMilliseconds);

            if (response.HttpStatusCode.IsSuccessCode())
            {
                if (desiredState) // we added the movie to the server's watchlist
                {
                    if (!Watchlist.MovieDetailModels.Select(movie_ => movie_.Id).Contains(movie.Id))
                        Watchlist.MovieDetailModels.Add(movie);
                }
                else // we removed the movie from the server's watchlist
                {
                    var movieToRemove = Watchlist.MovieDetailModels.FirstOrDefault(movie_ => movie_.Id == movie.Id);
                    Watchlist.MovieDetailModels.Remove(movieToRemove);
                }
            }
            else
                throw new Exception($"Could not update watchlist state, server responded with: {response.HttpStatusCode}");
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

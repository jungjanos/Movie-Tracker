using Ch9.ApiClient;
using Ch9.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Ch9.Utils
{
    public class FavoriteMoviesListService : INotifyPropertyChanged
    {
        private readonly ISettings _settings;
        private readonly ITmdbCachedSearchClient _tmdbCachedSearchClient;
        private readonly IMovieDetailModelConfigurator _movieDetailConfigurator;

        private SearchResult _favoriteMovies;
        public SearchResult FavoriteMovies
        {
            get => _favoriteMovies;
            set => SetProperty(ref _favoriteMovies, value);
        }

        public string SortBy;
        
        public FavoriteMoviesListService(
            ISettings settings,
            ITmdbCachedSearchClient tmdbCachedSearchClient,
            IMovieDetailModelConfigurator movieDetailConfigurator)
        {
            _settings = settings;
            _tmdbCachedSearchClient = tmdbCachedSearchClient;
            _movieDetailConfigurator = movieDetailConfigurator;

            SortBy = "created_at.desc";

            _favoriteMovies = new SearchResult
            {
                MovieDetailModels = new ObservableCollection<MovieDetailModel>(),
                Page = 0,
                TotalPages = 0,
                TotalResults = 0                
            };            
        }

        private void ClearFavoriteList()
        {
            FavoriteMovies.MovieDetailModels.Clear();
            FavoriteMovies.Page = 0;
            FavoriteMovies.TotalPages = 0;
            FavoriteMovies.TotalResults = 0;
        }

        /// <summary>
        /// Not allowed to throw. 
        /// Appends the movie collection in the "MovieDetailModels" property of the server response
        /// to the target's same named property. Updates the targets page and result counters from the serverResponse 
        /// </summary>
        /// <param name="targetList">this is the public observered collection wich is updated with data from the server's response</param>
        /// <param name="serverResponse">contains the server's response, containing movies to append to the observed collection and page/result counter</param>
        private void AppendResult(SearchResult targetList, SearchResult serverResponse)
        {
            if (serverResponse.MovieDetailModels.Count > 0)
            {
                _movieDetailConfigurator.SetImageSrc(serverResponse.MovieDetailModels);
                _movieDetailConfigurator.SetGenreNamesFromGenreIds(serverResponse.MovieDetailModels);

                foreach (MovieDetailModel movie in serverResponse.MovieDetailModels)
                    targetList.MovieDetailModels.Add(movie);
                    

                targetList.Page = serverResponse.Page;
                targetList.TotalPages = serverResponse.TotalPages;
                targetList.TotalResults = serverResponse.TotalResults;
            }                
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
            

            GetFavoriteMoviesResult getFavoriteList = await _tmdbCachedSearchClient.GetFavoriteMovies(sortBy: SortBy, page: 1, retryCount: retryCount, delayMilliseconds: delayMilliseconds);
            if (!getFavoriteList.HttpStatusCode.IsSuccessCode())
                throw new Exception($"Could not refresh favorite list, TMDB server responded with {getFavoriteList.HttpStatusCode}");

            SearchResult moviesOnFavoriteList = JsonConvert.DeserializeObject<SearchResult>(getFavoriteList.Json);

            AppendResult(FavoriteMovies, moviesOnFavoriteList);
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


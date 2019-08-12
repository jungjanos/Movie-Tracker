using Ch9.ApiClient;
using Ch9.Models;
using Ch9.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Ch9.Services
{
    public class CustomListsService : INotifyPropertyChanged
    {
        private readonly ISettings _settings;
        private readonly ITmdbCachedSearchClient _tmdbCachedSearchClient;
        private readonly IMovieDetailModelConfigurator _movieDetailConfigurator;
        private readonly Command _refreshActiveCustomListCommand;


        private ObservableCollection<MovieListModel> _usersCustomLists;
        public ObservableCollection<MovieListModel> UsersCustomLists
        {
            get => _usersCustomLists;
            set => SetProperty(ref _usersCustomLists, value);
        }

        private MovieListModel _selectedCustomList;
        public MovieListModel SelectedCustomList
        {
            get => _selectedCustomList;
            set
            {
                if (SetProperty(ref _selectedCustomList, value))
                {
                    _settings.ActiveMovieListId = SelectedCustomList?.Id;
                    _refreshActiveCustomListCommand.Execute(null);
                }
            }
        }

        public CustomListsService(
            ISettings settings,
            ITmdbCachedSearchClient tmdbCachedSearchClient,
            IMovieDetailModelConfigurator movieDetailConfigurator
            )
        {
            _settings = settings;
            _tmdbCachedSearchClient = tmdbCachedSearchClient;
            _movieDetailConfigurator = movieDetailConfigurator;

            _refreshActiveCustomListCommand = new Command(async () =>
            {
                if (SelectedCustomList != null)
                {
                    try
                    {
                        await UpdateSingleCustomList(SelectedCustomList.Id);
                    }
                    catch { }
                }
            });

            UsersCustomLists = new ObservableCollection<MovieListModel>();
        }

        private void ClearLists()
        {
            UsersCustomLists?.Clear();
            SelectedCustomList = null;
        }

        /// <summary>
        /// Can throw. 
        /// Tries to query the TMDB server for an update of the list of the users movie lists
        /// If fails the client side collection of the list of movie lists is reset.
        /// </summary>        
        public async Task UpdateCustomLists(int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = false)
        {
            if (!_settings.HasTmdbAccount)
            {
                ClearLists();
                throw new Exception("Account error: user is not signed in");
            }

            try
            {
                GetListsResult getLists = await _tmdbCachedSearchClient.GetLists(retryCount: retryCount, delayMilliseconds: delayMilliseconds, fromCache: fromCache);

                if (!getLists.HttpStatusCode.IsSuccessCode())
                {
                    ClearLists();
                    throw new Exception($"When trying to fetch user's custom lists, server responded with error code: {getLists.HttpStatusCode}");
                }

                MovieListModel[] movieLists = JsonConvert.DeserializeObject<GetListsModel>(getLists.Json).MovieLists;

                var selectedListBackup = SelectedCustomList;
                SelectedCustomList = null;
                Utils.Utils.UpdateListviewCollection(UsersCustomLists, movieLists, new MovieListModelComparer());

                if (selectedListBackup != null)
                    SelectedCustomList = UsersCustomLists.Contains(selectedListBackup) ? selectedListBackup : UsersCustomLists.FirstOrDefault();
                else
                    SelectedCustomList = UsersCustomLists.FirstOrDefault();
            }
            catch (Exception ex)
            {
                ClearLists();
                throw new Exception($"Exception happened when trying to fetch user's custom lists. Message: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Can throw. 
        /// Tries to create a new public custom movie list on the TMDB server. 
        /// If successfull, the new list is added to the client side list collection and it is made active. 
        /// </summary>
        /// <param name="name">list name</param>
        /// <param name="description">optional: list description</param>
        public async Task AddAndMakeActiveCustomList(string name, string description, int retryCount = 1, int delayMilliseconds = 1000)
        {
            if (!_settings.HasTmdbAccount)
                throw new Exception("Account error: user is not signed in");

            CreateListResult result = await _tmdbCachedSearchClient.CreateList(name, description, _settings.SearchLanguage ?? "en", retryCount, delayMilliseconds);
            if (result.HttpStatusCode.IsSuccessCode())
            {
                ListCrudResponseModel newListResponse = JsonConvert.DeserializeObject<ListCrudResponseModel>(result.Json);

                MovieListModel newList = new MovieListModel
                {
                    Id = newListResponse.ListId,
                    Name = name,
                    Description = description,
                };

                UsersCustomLists.Add(newList);
                SelectedCustomList = newList;
            }
            else
                throw new Exception($"Server responded with {result.HttpStatusCode}");
        }

        /// <summary>
        /// Can thow. 
        /// Tries to remove the currently selected movie list
        /// </summary>
        public async Task RemoveActiveCustomList(int retryCount = 1, int delayMilliseconds = 1000)
        {
            if (!_settings.HasTmdbAccount)
            {
                ClearLists();
                throw new Exception("Account error: user is not signed in");
            }

            if (SelectedCustomList == null)
                throw new Exception($"Error when trying to remove active custom list: there is no list selected");

            var listToDelete = SelectedCustomList;
            DeleteListResult result = await _tmdbCachedSearchClient.DeleteList(SelectedCustomList.Id, retryCount, delayMilliseconds);

            // Tmdb Web API has a glitch here: Http.500 denotes "success" here...
            bool success = result.HttpStatusCode.Is500Code();
            if (success)
            {
                SelectedCustomList = null;
                UsersCustomLists.Remove(listToDelete);
                SelectedCustomList = UsersCustomLists.FirstOrDefault();
            }
            else
                throw new Exception($"Server responded with error code: {result.HttpStatusCode}");
        }

        /// <summary>
        /// Can throw. 
        /// Tries to remove a movie from the currently selected custom movie list
        /// </summary>
        public async Task RemoveMovieFromActiveList(int movieId, int retryCount = 1, int delayMilliseconds = 1000)
        {
            if (!_settings.HasTmdbAccount)
                throw new Exception("Account error: user is not signed in");

            if (SelectedCustomList == null)
                throw new Exception("Error when trying to remove movie from active list: no custom list is selected");

            if (SelectedCustomList.Movies == null)
                throw new Exception("Error when trying to remove movie from active list: selected custom list is not a valid public list");

            var movieToRemove = SelectedCustomList.Movies.FirstOrDefault(movie => movie.Id == movieId);
            if (movieToRemove == null)
                throw new Exception("Error when trying to remove movie from active list: to be removed movie is invalid or not on the list");

            RemoveMovieResult result = await _tmdbCachedSearchClient.RemoveMovie(SelectedCustomList.Id, movieId, retryCount, delayMilliseconds);

            if (result.HttpStatusCode.IsSuccessCode())
                SelectedCustomList.Movies.Remove(movieToRemove);
            else
                throw new Exception($"Error when trying to remove movie from active list, server responded with error code: {result.HttpStatusCode}");
        }

        /// <summary>
        /// Can throw. 
        /// Tries to add a movie to the currently selected custom movie list
        /// </summary>
        public async Task AddMovieToActiveList(MovieDetailModel movie, int retryCount = 1, int delayMilliseconds = 1000)
        {
            if (!_settings.HasTmdbAccount)
                throw new Exception("Account error: user is not signed in");

            if (SelectedCustomList == null)
                throw new Exception("Error when trying to add movie to active list: no custom list is selected");

            if (SelectedCustomList.Movies == null)
                throw new Exception("Error when trying to add movie to active list: selected custom list is not a valid public list");

            if (CheckIfMovieIsOnActiveList(movie.Id) == true)
                throw new Exception("Error when trying to add movie to active list: to be added movie is already on the list");

            AddMovieResult result = await _tmdbCachedSearchClient.AddMovie(SelectedCustomList.Id, movie.Id, retryCount, delayMilliseconds);

            if (result.HttpStatusCode.IsSuccessCode())
                SelectedCustomList.Movies.Add(movie);
            else
                throw new Exception($"Error when trying to add movie to active list, server responded with error code: {result.HttpStatusCode}");
        }

        /// <summary>
        /// Can throw. 
        /// Updates the currently selected custom movie list. 
        /// </summary>
        public async Task UpdateSingleCustomList(int listId, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = false)
        {
            if (!_settings.HasTmdbAccount)
            {
                ClearLists();
                throw new Exception("Account error: user is not signed in");
            }

            MovieListModel updateTarget = UsersCustomLists.FirstOrDefault(list => list.Id == listId);
            if (updateTarget == null)
                throw new Exception($"Error when trying to update active movie list: couldn't find the to target list with id={listId} in your custom lists");

            GetListDetailsResult listDetailResponse = await _tmdbCachedSearchClient.GetListDetails(listId, _settings.SearchLanguage, retryCount, delayMilliseconds, fromCache);
            if (!listDetailResponse.HttpStatusCode.IsSuccessCode())
                throw new Exception($"Error when trying to update active movie list with id={listId}. Server responded with {listDetailResponse.HttpStatusCode} code");

            MovieListModel movieListDetails = JsonConvert.DeserializeObject<MovieListModel>(listDetailResponse.Json);
            HydrateMovieList(updateTarget, movieListDetails);
        }

        /// <summary>
        /// Not allowed to throw. 
        /// Returns null if user is not logged in or no custom movie list is currently selected. 
        /// Returns false if the queried movie is not on the active list. 
        /// Returns true if the queried movie is on the active list. 
        /// </summary> 
        public bool? CheckIfMovieIsOnActiveList(int movieId)
        {
            if (!_settings.HasTmdbAccount)
                return null;

            if (SelectedCustomList == null)
                return null;
            else
                return SelectedCustomList.Movies?.FirstOrDefault(movie => movie.Id == movieId) != null;
        }

        private void HydrateMovieList(MovieListModel target, MovieListModel source)
        {
            if (target.Id != source.Id)
                throw new ArgumentException($"source and target  {nameof(MovieListModel)}  Id properties must be equal");

            if (source.Movies == null)
                throw new ArgumentException($"source movie list must not be empty");

            target.Description = source.Description;
            target.FavoriteCount = source.FavoriteCount;
            target.ItemCount = source.ItemCount;
            target.Iso639 = source.Iso639;
            target.ListType = source.ListType;
            target.Name = source.Name;
            target.PosterPath = source.PosterPath;

            _movieDetailConfigurator.SetImageSrc(source.Movies);
            _movieDetailConfigurator.SetGenreNamesFromGenreIds(source.Movies);

            if (target.Movies == null)
                target.Movies = source.Movies;
            else
                Utils.Utils.UpdateListviewCollection(target.Movies, source.Movies, new MovieModelComparer());
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

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
using Xamarin.Forms;

namespace Ch9.Utils
{
    // encapsulates all the Movie list based logic and operations for affected ViewModels
    // Lists include: Custom user created lists, Server provided default lists (favorite list, watchlist etc..)
    // This include: getting Movie List Ids from cache at startup    
    // Fetching List details from server
    // Adding and removing Lists
    // Adding and removing Movies to/from Lists
    //
    public class UsersMovieListsService : INotifyPropertyChanged
    {
        private readonly ISettings _settings;
        private readonly ITmdbCachedSearchClient _tmdbCachedSearchClient;
        private readonly IMovieDetailModelConfigurator _movieDetailConfigurator;
        private readonly Application _xamarinApplication;

        public Task<bool> Initializer { get; private set; }

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
                    _settings.MovieIdsOnActiveList = SelectedCustomList?.Movies?.Select(movie => movie.Id)?.ToArray();
                }
            }
        }

        public UsersMovieListsService(
            ISettings settings,
            ITmdbCachedSearchClient tmdbCachedSearchClient,
            IMovieDetailModelConfigurator movieDetailConfigurator,
            Application xamarinApplication = null
            )
        {
            _settings = settings;
            _tmdbCachedSearchClient = tmdbCachedSearchClient;
            _movieDetailConfigurator = movieDetailConfigurator;
            _xamarinApplication = xamarinApplication;
            UsersCustomLists = new ObservableCollection<MovieListModel>();

            Initializer = Initialize();
        }

        private async Task<bool> Initialize() => await RefreshUsersCustomLists();

        public async Task RefreshLists() => await RefreshUsersCustomLists();
        private async Task<bool> RefreshUsersCustomLists()
        {
            MovieListModel[] fetchedUserLists = await GetUsersLists(retries: 3, retryDelay: 1000, fromCache: false);

            if (fetchedUserLists != null)
            {
                foreach (var list in fetchedUserLists)
                {
                    if (!UsersCustomLists.Select(l => l.Id).Contains(list.Id))
                        UsersCustomLists.Add(list);
                }

                foreach (var list in UsersCustomLists)
                {
                    if (!fetchedUserLists.Select(l => l.Id).Contains(list.Id))
                    {
                        if (list == SelectedCustomList)
                            SelectedCustomList = null;

                        UsersCustomLists.Remove(list);
                    }
                }

                if (SelectedCustomList == null)
                    SelectedCustomList = UsersCustomLists.FirstOrDefault();
                return true;
            }

            UsersCustomLists = new ObservableCollection<MovieListModel>();
            SelectedCustomList = null;
            return false;
        }


        // TODO : Refactor this method into a API Call Orchestrator 
        // We refresh the cache containing the users movie lists if the user has a validated Tmdb account
        private async Task<MovieListModel[]> GetUsersLists(int retries, int retryDelay, bool fromCache)
        {
            List<MovieListModel> result = null;

            if (_settings.HasTmdbAccount)
            {
                result = new List<MovieListModel>();

                MovieListModel[] movieLists;

                try
                {
                    GetListsResult getLists = await _tmdbCachedSearchClient.GetLists(retryCount: retries, delayMilliseconds: retryDelay, fromCache: fromCache);

                    if (!getLists.HttpStatusCode.IsSuccessCode())
                        return null;

                    movieLists = JsonConvert.DeserializeObject<GetListsModel>(getLists.Json).MovieLists;
                }
                catch { return null; };

                var getListDetailsCollection = movieLists.Select(list => new
                {
                    List = list,
                    ListDetailTask = _tmdbCachedSearchClient.GetListDetails(list.Id, retryCount: 3, delayMilliseconds: 1000, fromCache: fromCache)
                }).ToArray();

                try
                {
                    await Task.WhenAll(getListDetailsCollection.Select(element => element.ListDetailTask));
                }
                catch { return null; }

                foreach (var listDetail in getListDetailsCollection)
                {
                    var detailResult = await listDetail.ListDetailTask;
                    if (detailResult.HttpStatusCode.IsSuccessCode())
                    {
                        try
                        {
                            listDetail.List.Movies = JsonConvert.DeserializeObject<MovieListModel>(detailResult.Json).Movies;

                            _movieDetailConfigurator.SetImageSrc(listDetail.List.Movies);
                            _movieDetailConfigurator.SetGenreNamesFromGenreIds(listDetail.List.Movies);
                            result.Add(listDetail.List);
                        }
                        catch { }
                    }
                }

                return result.ToArray();
            }
            else
                return null;
        }


        // Provides a best effort check if the movie is on the users selected active list:
        // behavior:
        // 1, if user has no account => returns null
        //
        // 2, if user has account:                 
        // 2.1 Only if the Settings dictionary does not contain a pre-stored active list => returns null
        // 2.2 If the settings dictionary contains an active pre-stored list => returns whether movieId could be found on pre stored list

        public bool? CheckIfMovieIsAlreadyOnActiveList(int movieId)
        {
            if (!_settings.HasTmdbAccount)
                return null;

            return _settings.MovieIdsOnActiveList?.Contains(movieId);
        }

        public async Task AddMovieToUsersActiveCustomList(MovieDetailModel movie)
        {
            if (CheckIfMovieIsAlreadyOnActiveList(movie.Id) != false)
                return;

            if (!_settings.HasTmdbAccount)
                return;

            await Initializer;

            if (_settings.ActiveMovieListId == null)
                return;

            AddMovieResult result = await _tmdbCachedSearchClient.AddMovie(_settings.ActiveMovieListId.Value, movie.Id, retryCount: 3, delayMilliseconds: 1000);

            if (result.HttpStatusCode.IsSuccessCode())
            {
                _settings.MovieIdsOnActiveList = _settings.MovieIdsOnActiveList.Union(Enumerable.Repeat(movie.Id, 1)).ToArray();
                SelectedCustomList.Movies.Add(movie); //TODO : OnPropertyChanged required, or does it propagete?? 
            }
        }

        public async Task RemoveMovieFromUsersActiveCustomList(int movieId)
        {
            if (!_settings.HasTmdbAccount)
                return;

            await Initializer;

            if (_settings.ActiveMovieListId == null)
                return;

            RemoveMovieResult result = await _tmdbCachedSearchClient.RemoveMovie(_settings.ActiveMovieListId.Value, movieId, 3, 1000);
            if (result.HttpStatusCode.IsSuccessCode())
            {
                _settings.MovieIdsOnActiveList = SelectedCustomList?.Movies?.Select(movie => movie.Id)?.ToArray();
                var toRemove = SelectedCustomList?.Movies?.FirstOrDefault(movie => movie.Id == movieId);
                SelectedCustomList?.Movies?.Remove(toRemove); //TODO : OnPropertyChanged required, or does it propagete?? 
            }
        }

        public async Task RemoveSelectedMovieList()
        {
            if (!_settings.HasTmdbAccount)
                return;

            await Initializer;

            if (_settings.ActiveMovieListId == null)
                return;

            var listToDelete = SelectedCustomList;
            

            DeleteListResult result = await _tmdbCachedSearchClient.DeleteList(_settings.ActiveMovieListId.Value, retryCount: 3, delayMilliseconds: 1000);

            // Tmdb Web API has a glitch here: Http.500 denotes "success" here...
            bool success = result.HttpStatusCode.Is500Code();
            if (success)
            {
                SelectedCustomList = null;
                UsersCustomLists.Remove(listToDelete);
                SelectedCustomList = UsersCustomLists.FirstOrDefault();
            }
        }

        public async Task AddList(string name, string description)
        {
            if (!_settings.HasTmdbAccount)
                return;

            await Initializer;

            CreateListResult result = await _tmdbCachedSearchClient.CreateList(name, description, retryCount: 3, delayMilliseconds: 1000);
            if (result.HttpStatusCode.IsSuccessCode())
            {
                await RefreshUsersCustomLists();
                int newListId = JsonConvert.DeserializeObject<ListCrudResponseModel>(result.Json).ListId;
                SelectedCustomList = UsersCustomLists.FirstOrDefault(x => x.Id == newListId);
            }
        }


        public async Task ResetState()
        {
            await DeleteLocalDataAndObjects();
            Initializer = Initialize();
        }
        private async Task DeleteLocalDataAndObjects()
        {
            UsersCustomLists = null;
            SelectedCustomList = null;
            await _xamarinApplication?.SavePropertiesAsync();
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

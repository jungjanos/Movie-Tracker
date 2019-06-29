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
    public class UsersMovieListsService2 : INotifyPropertyChanged
    {
        private readonly ISettings _settings;
        private readonly ITmdbCachedSearchClient _tmdbCachedSearchClient;
        private readonly IMovieDetailModelConfigurator _movieDetailConfigurator;
        private readonly Application _xamarinApplication;
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

        public UsersMovieListsService2(
            ISettings settings,
            ITmdbCachedSearchClient tmdbCachedSearchClient,
            IMovieDetailModelConfigurator movieDetailConfigurator,
            Application xamarinApplication = null)
        {
            _settings = settings;
            _tmdbCachedSearchClient = tmdbCachedSearchClient;
            _movieDetailConfigurator = movieDetailConfigurator;
            _xamarinApplication = xamarinApplication;
            _refreshActiveCustomListCommand = new Command(async () =>
            {
                if (SelectedCustomList != null)
                    await UpdateSingleCustomList(SelectedCustomList.Id);
            });


            UsersCustomLists = new ObservableCollection<MovieListModel>();
        }

        public async Task Initialize()
        {
            await UpdateCustomLists(1, 1000, false);
            if (_settings.ActiveMovieListId.HasValue)
                SelectedCustomList = UsersCustomLists.FirstOrDefault(list => list.Id == _settings.ActiveMovieListId);

            if (SelectedCustomList != null)
                await UpdateSingleCustomList(SelectedCustomList.Id, 1, 1000, false);
        }


        private void ClearList()
        {
            UsersCustomLists?.Clear();
            SelectedCustomList = null;
        }

        public async Task UpdateCustomLists(int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = false)
        {
            if (!_settings.HasTmdbAccount)
            {
                ClearList();
                return;
            }

            try
            {
                GetListsResult getLists = await _tmdbCachedSearchClient.GetLists(retryCount: retryCount, delayMilliseconds: delayMilliseconds, fromCache: fromCache);

                if (!getLists.HttpStatusCode.IsSuccessCode())
                {
                    ClearList();
                    return;
                }

                MovieListModel[] movieLists = JsonConvert.DeserializeObject<GetListsModel>(getLists.Json).MovieLists;

                var selectedListBackup = SelectedCustomList;
                SelectedCustomList = null;
                Utils.UpdateListviewCollection(UsersCustomLists, movieLists, new MovieListModelComparer());

                if (selectedListBackup != null)
                    SelectedCustomList = UsersCustomLists.Contains(selectedListBackup) ? selectedListBackup : UsersCustomLists.FirstOrDefault();
            }
            catch
            {
                ClearList();
            }
        }

        public async Task RemoveActiveCustomList(int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = false)
        {
            if (!_settings.HasTmdbAccount)
            {
                ClearList();
                return;
            }

            if (SelectedCustomList == null)
                return;

            var listToDelete = SelectedCustomList;

            DeleteListResult result = await _tmdbCachedSearchClient.DeleteList(SelectedCustomList.Id, retryCount: 1, delayMilliseconds: 1000);

            // Tmdb Web API has a glitch here: Http.500 denotes "success" here...
            bool success = result.HttpStatusCode.Is500Code();
            if (success)
            {
                SelectedCustomList = null;
                UsersCustomLists.Remove(listToDelete);
                SelectedCustomList = UsersCustomLists.FirstOrDefault();
            }
        }

        public async Task UpdateSingleCustomList(int listId, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = false)
        {
            if (!_settings.HasTmdbAccount)
            {
                ClearList();
                return;
            }

            MovieListModel updateTarget = UsersCustomLists.FirstOrDefault(list => list.Id == listId);
            if (updateTarget == null)
                return;

            try
            {
                GetListDetailsResult listDetailResponse = await _tmdbCachedSearchClient.GetListDetails(listId, _settings.SearchLanguage, retryCount, delayMilliseconds, fromCache);
                if (!listDetailResponse.HttpStatusCode.IsSuccessCode())
                    return;

                MovieListModel movieListDetails = JsonConvert.DeserializeObject<MovieListModel>(listDetailResponse.Json);
                HydrateMovieList(updateTarget, movieListDetails);
            }
            catch { }
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

            if (target.Movies == null)
                target.Movies = source.Movies;
            else
            {
                _movieDetailConfigurator.SetImageSrc(source.Movies);
                _movieDetailConfigurator.SetGenreNamesFromGenreIds(source.Movies);
                Utils.UpdateListviewCollection(target.Movies, source.Movies, new MovieModelComparer());
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

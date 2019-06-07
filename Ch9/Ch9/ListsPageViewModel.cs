using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Ch9.ApiClient;
using Ch9.Models;
using Ch9.Utils;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace Ch9
{
    // Remark:
    // Up to this point in development I didnt use MVVM 
    // At this point the extreme slowness of the Android emulator and 
    // the annoying debugging of Xamarin related pieces 
    // Combined with my inexperience of XAML has forced me 
    // to make a clearer separation of Xamarin page display (framework) from page behavior
    // to be able to develop and test page behavior separatelly from Xamarin framework as POCO
    // classes
    // The goal is to make the development more rapid and not to achieve MVVM purism
    public class ListsPageViewModel : INotifyPropertyChanged
    {
        public string DebugVerison { get; } = "0.0.25";

        private readonly ISettings _settings;
        private readonly ITmdbCachedSearchClient _cachedSearchClient;
        private readonly IMovieDetailModelConfigurator _movieDeatilConfigurator;
        private readonly IPageService _pageService;
        // TODO : this field should be removed (not necessary with current initialization model)
        private bool _initialized = false;

        private ObservableCollection<MovieListModel> _movieLists;
        public ObservableCollection<MovieListModel> MovieLists
        {
            get => _movieLists;
            set => SetProperty(ref _movieLists, value);
        }

        private MovieListModel _selectedList;
        // When the user changes the active list on the UI, the
        // Id of the active list and the Id-s of the Movies on the active list have to be 
        // updated in the local store

        // TODO : investigate whether Application.SavePropertiesAsync() call makes sense here...
        public MovieListModel SelectedList
        {
            get => _selectedList;
            set
            {
                if (SetProperty(ref _selectedList, value))
                {
                    _settings.ActiveMovieListId = SelectedList?.Id;
                    _settings.MovieIdsOnActiveList = SelectedList?.Movies?.Select(movie => movie.Id)?.ToArray();
                }
            }
        }

        private MovieDetailModel _selectedMovie;
        public MovieDetailModel SelectedMovie
        {
            get => _selectedMovie;
            set => SetProperty(ref _selectedMovie, value);
        }

        private bool _isRefreshing = false;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }
        public Command RefreshCommand { get; private set; }
        public Command MovieInfoCommand { get; private set; }
        public Command RemoveListCommand { get; private set; }
        public Command AddListCommand { get; private set; }

        public ListsPageViewModel(
            ISettings settings,
            ITmdbCachedSearchClient cachedSearchClient,
            IMovieDetailModelConfigurator movieDeatilConfigurator,
            IPageService pageService)
        {
            MovieLists = new ObservableCollection<MovieListModel>();
            _settings = settings;
            _cachedSearchClient = cachedSearchClient;
            _movieDeatilConfigurator = movieDeatilConfigurator;
            _pageService = pageService;

            AddListCommand = new Command(async () =>
            {
                if (!_settings.HasTmdbAccount)
                {
                    await _pageService.DisplayAlert(
                        "Authentication error",
                        "To use this feature, you need to sign in with your TMDB account",
                        "Ok");
                    return;
                }
                await _pageService.PushAsync(new AddListPageViewModel(this));
            });
            RefreshCommand = new Command(async () => await RefreshMovieList());
            MovieInfoCommand = new Command(async () => await OpenMovieDetailPage());
            RemoveListCommand = new Command(async () => await RemoveList());
        }

        public async Task Initialize()
        {
            if (_initialized)
                return;

            if (MovieLists.Count == 0)
            {
                MovieListModel[] fetchedUserLists = await GetUsersLists(retries: 3, retryDelay: 1000, fromCache: true);

                if (fetchedUserLists != null)
                {
                    MovieLists = new ObservableCollection<MovieListModel>(fetchedUserLists);

                    SelectedList = MovieLists.FirstOrDefault(x => x.Id == _settings.ActiveMovieListId) ??
                        MovieLists.FirstOrDefault();

                    _initialized = true;
                }
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

        // TODO : Refactor this method into a API Call Orchestrator 
        // We refresh the cache containing the users movie lists if the user has a validated Tmdb account
        private async Task<MovieListModel[]> GetUsersLists(int retries, int retryDelay, bool fromCache)
        {
            List<MovieListModel> result = new List<MovieListModel>();

            if (_settings.HasTmdbAccount)
            {
                MovieListModel[] movieLists;

                try
                {
                    GetListsResult getLists = await _cachedSearchClient.GetLists(retryCount: retries, delayMilliseconds: retryDelay, fromCache: fromCache);

                    if (!getLists.HttpStatusCode.IsSuccessCode())
                        return null;

                    movieLists = JsonConvert.DeserializeObject<GetListsModel>(getLists.Json).MovieLists;
                }
                catch { return null; };

                var getListDetailsCollection = movieLists.Select(list => new
                                                {
                                                    List = list,
                                                    ListDetailTask = _cachedSearchClient
                                                    .GetListDetails(list.Id, retryCount: 3, delayMilliseconds: 1000, fromCache: fromCache)
                                                }).ToArray();

                await Task.WhenAll(getListDetailsCollection.Select(element => element.ListDetailTask));

                foreach (var listDetail in getListDetailsCollection)
                {
                    var detailResult = await listDetail.ListDetailTask;
                    if (detailResult.HttpStatusCode.IsSuccessCode())
                    {
                        try
                        {
                            listDetail.List.Movies = JsonConvert.DeserializeObject<MovieListModel>(detailResult.Json).Movies;

                            _movieDeatilConfigurator.SetImageSrc(listDetail.List.Movies);
                            _movieDeatilConfigurator.SetGenreNamesFromGenreIds(listDetail.List.Movies);
                            result.Add(listDetail.List);
                        }
                        catch { }
                    }
                }
            }
            return result.ToArray();
        }

        public async Task OpenMovieDetailPage()
        {
            if (SelectedMovie == null)
                return;

            await _pageService.PushAsync(SelectedMovie);
        }

        public async Task RefreshMovieList()
        {
            var selectedMovieListId = SelectedList?.Id;
            var selectedMovieId = SelectedMovie?.Id;

            MovieListModel[] fetchedUserLists = await GetUsersLists(retries: 3, retryDelay: 1000, fromCache: false);

            if (fetchedUserLists != null)
            {
                MovieLists.Clear();
                foreach (var list in fetchedUserLists)
                    MovieLists.Add(list);
                _initialized = true;

                SelectedList = MovieLists.FirstOrDefault(list => list.Id == selectedMovieListId) ??
                    MovieLists.FirstOrDefault();

                SelectedMovie = SelectedList?.Movies?.FirstOrDefault(movie => movie.Id == selectedMovieId);
            }
            IsRefreshing = false;
        }

        public async Task RemoveMovieFromList()
        {
            if (SelectedMovie == null || SelectedList == null)
                return;

            var movieToRemove = SelectedMovie;

            SelectedMovie = null;
            SelectedList.Movies.Remove(movieToRemove);
            RemoveMovieResult result = await _cachedSearchClient.RemoveMovie(SelectedList.Id, movieToRemove.Id, 3, 1000);
            if (result.HttpStatusCode.IsSuccessCode())
                _settings.MovieIdsOnActiveList = SelectedList?.Movies?.Select(movie => movie.Id)?.ToArray();
        }

        public async Task RemoveList()
        {
            if (SelectedList == null)
                return;

            MovieListModel listToDelete = SelectedList;

            if (listToDelete.Movies.Count > 0)
            {
                string dialogResult = await _pageService.DisplayActionSheet("List is not empty, confirm delete", "Cancel", "Ok");
                if (dialogResult == "Cancel")
                    return;
            }
            SelectedList = null;

            DeleteListResult result = await _cachedSearchClient.DeleteList(listToDelete.Id, retryCount: 3, delayMilliseconds: 1000);

            // Tmdb Web API has a glitch here: Http.500 denotes "success" here...
            bool success = result.HttpStatusCode.Is500Code();
            if (success)
            {
                MovieLists.Remove(listToDelete);
                SelectedList = MovieLists.FirstOrDefault();
            }
            else
                await _pageService.DisplayAlert("Error", $"{listToDelete.Name} : delete error, status code: {result.HttpStatusCode}", "Ok");
        }

        public async Task AddList(AddListPageViewModel addListPageViewModel)
        {
            CreateListResult result = await _cachedSearchClient.CreateList(addListPageViewModel.Name, addListPageViewModel.Description, retryCount: 3, delayMilliseconds: 1000);
            if (result.HttpStatusCode.IsSuccessCode())
            {
                await RefreshMovieList();
                int newListId = JsonConvert.DeserializeObject<ListCrudResponseModel>(result.Json).ListId;
                SelectedList = MovieLists.FirstOrDefault(x => x.Id == newListId);
            }
            else await _pageService.DisplayAlert("Error", $"Error adding list. Server responded with: {result.HttpStatusCode}", "Ok");
        }
    }
}

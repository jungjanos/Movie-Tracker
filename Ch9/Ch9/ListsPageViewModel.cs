using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Ch9.ApiClient;
using Ch9.Models;
using Ch9.Utils;
using Newtonsoft.Json;

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
        private ISettings _settings;
        private ITmdbCachedSearchClient _cachedSearchClient;
        private bool _initialized = false;

        private ObservableCollection<MovieListModel> _movieLists;
        public ObservableCollection<MovieListModel> MovieLists
        {
            get => _movieLists;
            set
            {
                if (SetProperty(ref _movieLists, value))
                    SelectedList = _movieLists.FirstOrDefault();
            } 
        }

        private MovieListModel _selectedList;       
        public MovieListModel SelectedList
        {
            get => _selectedList;
            set => SetProperty(ref _selectedList, value);
        }

        public ListsPageViewModel(ISettings settings, ITmdbCachedSearchClient cachedSearchClient)
        {
            MovieLists = new ObservableCollection<MovieListModel>();
            _settings = settings;
            _cachedSearchClient = cachedSearchClient;
        }

        public async Task Initialize()
        {
            if (_initialized)
                return;

            if (MovieLists.Count == 0)
            {
                MovieListModel[] fetchedUserLists = await GetUsersLists(3, 1000);

                if (fetchedUserLists != null)
                    MovieLists = new ObservableCollection<MovieListModel>(fetchedUserLists);

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
        private async Task<MovieListModel[]> GetUsersLists(int retries, int retryDelay)
        {
            List<MovieListModel> result = new List<MovieListModel>();

            if (_settings.HasTmdbAccount)
            {
                MovieListModel[] movieLists;

                try
                {
                    GetListsResult getLists = await _cachedSearchClient.GetLists(retryCount: 3, delayMilliseconds: 1000, fromCache: true);

                    if (!getLists.HttpStatusCode.IsSuccessCode())
                        return null;

                    movieLists = JsonConvert.DeserializeObject<GetListsModel>(getLists.Json).MovieLists;
                }
                catch { return null; };

                foreach (var list in movieLists)
                {
                    GetListDetailsResult getListDetails = await _cachedSearchClient.GetListDetails(list.Id, retryCount: 3, delayMilliseconds: 1000, fromCache: true);

                    if (getListDetails.HttpStatusCode.IsSuccessCode())
                    {
                        try
                        {
                            list.Movies = JsonConvert.DeserializeObject<MovieListModel>(getListDetails.Json).Movies;
                            result.Add(list);
                        }
                        catch { }
                    }
                }
            }
            return result.ToArray();
        }
    }
}

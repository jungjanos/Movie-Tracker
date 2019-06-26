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
    public class ListsPageViewModel2 : INotifyPropertyChanged
    {
        public string DebugVerison { get; } = "0.0.26";
        
        private readonly ITmdbCachedSearchClient _cachedSearchClient;
        private readonly IPageService _pageService;
        private readonly UsersMovieListsService _usersMovieListsService;

        private ObservableCollection<MovieListModel> _movieLists;
        public ObservableCollection<MovieListModel> MovieLists
        {
            get => _movieLists;
            set => SetProperty(ref _movieLists, value);
        }

        private MovieListModel _selectedList;
        public MovieListModel SelectedList
        {
            get => _selectedList;
            set => SetProperty(ref _selectedList, value);               
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

        public ListsPageViewModel2(
            ITmdbCachedSearchClient cachedSearchClient,
            IPageService pageService,
            UsersMovieListsService usersMovieListsService
            )
        {                       
            _cachedSearchClient = cachedSearchClient;
            _pageService = pageService;
            _usersMovieListsService = usersMovieListsService;
            AddListCommand = new Command(async () =>{ });
            RefreshCommand = new Command(async () => { });
            MovieInfoCommand = new Command(async () => { });
            RemoveListCommand = new Command(async () => { });         
        }

        public async Task Initialize()
        {
            await _usersMovieListsService.Initializer;
            MovieLists = _usersMovieListsService.UsersCustomLists;
            SelectedList = _usersMovieListsService.SelectedCustomList;
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

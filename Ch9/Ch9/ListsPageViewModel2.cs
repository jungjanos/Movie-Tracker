using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Ch9.ApiClient;
using Ch9.Models;
using Ch9.Utils;
using Xamarin.Forms;

namespace Ch9
{
    public class ListsPageViewModel2 : INotifyPropertyChanged
    {
        public string DebugVerison { get; } = "0.0.26";

        public IPageService PageService
        {
            get => _pageService;
            private set => SetProperty(ref _pageService, value);
        }
        private UsersMovieListsService _usersMovieListsService;
        public UsersMovieListsService UsersMovieListsService
        {
            get => _usersMovieListsService;
            set => SetProperty(ref _usersMovieListsService, value);
        }
        private readonly ISettings _settings;
        //private ObservableCollection<MovieListModel> _movieLists;
        //public ObservableCollection<MovieListModel> MovieLists
        //{
        //    get => _movieLists;
        //    set => SetProperty(ref _movieLists, value);
        //}

        //private MovieListModel _selectedList;
        //public MovieListModel SelectedList
        //{
        //    get => _selectedList;
        //    set => SetProperty(ref _selectedList, value);               
        //}

        private MovieDetailModel _selectedMovie;
        public MovieDetailModel SelectedMovie
        {
            get => _selectedMovie;
            set => SetProperty(ref _selectedMovie, value);
        }

        private bool _isRefreshing = false;
        private IPageService _pageService;

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
            UsersMovieListsService usersMovieListsService,
            ISettings settings,
            ITmdbCachedSearchClient cachedSearchClient,
            IPageService pageService
            )
        {
            PageService = pageService;
            UsersMovieListsService = usersMovieListsService;
            _settings = settings;
            AddListCommand = new Command(async () =>
            {
                if (!_settings.HasTmdbAccount)
                {
                    await PageService.DisplayAlert(
                        "Authentication error",
                        "To use this feature, you need to sign in with your TMDB account",
                        "Ok");
                    return;
                }
                await PageService.PushAsync(new AddListPageViewModel(this));
            });
            RefreshCommand = new Command(async () => 
            {
                await UsersMovieListsService.RefreshLists();
                OnPropertyChanged(nameof(UsersMovieListsService));
                OnPropertyChanged(nameof(UsersMovieListsService.SelectedCustomList));
            });
            //MovieInfoCommand = new Command(async () => { });
            RemoveListCommand = new Command(async () => await UsersMovieListsService.RemoveSelectedMovieList());         
        }

        public async Task Initialize()
        {
            await UsersMovieListsService.Initializer;
            OnPropertyChanged(nameof(UsersMovieListsService));
            OnPropertyChanged(nameof(UsersMovieListsService));
        }


        public async Task AddList(AddListPageViewModel addListPageViewModel)
        {
            await UsersMovieListsService.AddList(addListPageViewModel.Name, addListPageViewModel.Description);                                    
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

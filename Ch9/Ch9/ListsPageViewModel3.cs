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
    public class ListsPageViewModel3 : INotifyPropertyChanged
    {
        private readonly ISettings _settings;
        private IPageService _pageService;

        public string DebugVerison { get; } = "0.0.27";

        private UsersMovieListsService2 _usersMovieListsService2;
        public UsersMovieListsService2 UsersMovieListsService2
        {
            get => _usersMovieListsService2;
            set => SetProperty(ref _usersMovieListsService2, value);
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
        public Command RefreshListCommand { get; private set; }
        public Command MovieInfoCommand { get; private set; }
        public Command RemoveListCommand { get; private set; }
        public Command AddListCommand { get; private set; }


        public ListsPageViewModel3(
            UsersMovieListsService2 usersMovieListsService2,
            ISettings settings,
            ITmdbCachedSearchClient cachedSearchClient,
            IPageService pageService)
        {
            _pageService = pageService;
            UsersMovieListsService2 = usersMovieListsService2;
            _settings = settings;

            RefreshCommand = new Command(async () => {
                IsRefreshing = true;
                await UsersMovieListsService2.UpdateCustomLists();
                IsRefreshing = false;
            });

            RefreshListCommand = new Command(async () =>
            {
                IsRefreshing = true;
                if (UsersMovieListsService2.SelectedCustomList != null)
                    await UsersMovieListsService2.UpdateSingleCustomList(UsersMovieListsService2.SelectedCustomList.Id);
                IsRefreshing = false;
            });

            RemoveListCommand = new Command(async () => await UsersMovieListsService2.RemoveActiveCustomList());
            AddListCommand = new Command(async () => await _pageService.PushAsync(new AddListPageViewModel(this)));
        }

        public async Task AddList(AddListPageViewModel addListPageViewModel)
        {
            await UsersMovieListsService2.AddAndMakeActiveCustomList(addListPageViewModel.Name, addListPageViewModel.Description);
        }

        public async Task Initialize() => await UsersMovieListsService2.Initialize();

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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Ch9.Models;
using Ch9.Services;
using Xamarin.Forms;

namespace Ch9.ViewModels
{
    public class ListsPageViewModel3 : INotifyPropertyChanged
    {
        private readonly IPageService _pageService;

        private UsersMovieListsService2 _usersMovieListsService2;
        public UsersMovieListsService2 UsersMovieListsService2
        {
            get => _usersMovieListsService2;
            set => SetProperty(ref _usersMovieListsService2, value);
        }

        /// <summary>
        /// 0 = not defined (do not reset to it), 1 = CUSTOM, 2 = FAVORITES 3 = WATCHLIST
        /// </summary>
        private int _selectedListType = 0;
        public int SelectedListType
        {
            get => _selectedListType;
            set => SetProperty(ref _selectedListType, value);
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

        #region VIEW_SELECTOR_COMMANDS        
        public Command WatchlistViewSelectorCommand { get; private set; }
        public Command FavoritesListViewSelectorCommand { get; private set; }
        public Command CustomListsViewSelectorCommand { get; private set; }        
        #endregion

        #region CUSTOM_LISTS_COMMANDS
        public Command RemoveCustomListCommand { get; private set; }
        public Command AddCustomListCommand { get; private set; }
        public Command RefreshCustomCommand { get; private set; }
        public Command RefreshCustomListCommand { get; private set; }
        public Command RemoveMovieFromCustomListCommand { get; private set; }
        #endregion

        #region FAVORITE_LIST_COMMANDS
        public Command RefreshFavoriteListCommand { get; private set; }
        public Command LoadNextFavoritesPageCommand { get; private set; }
        #endregion
        #region WATCHLIST_COMMANDS
        public Command RefreshWatchlistCommand { get; private set; }
        public Command LoadNextWatchlistPageCommand { get; private set; }
        #endregion
        public ICommand OnItemTappedCommand { get; private set; }


        public ListsPageViewModel3(
            UsersMovieListsService2 usersMovieListsService2,
            IPageService pageService)
        {
            _pageService = pageService;
            UsersMovieListsService2 = usersMovieListsService2;            

            WatchlistViewSelectorCommand = new Command(async () =>
            {
                if (!(0 < UsersMovieListsService2.WatchlistService.Watchlist.MovieDetailModels.Count))
                {
                    IsRefreshing = true;
                    try
                    {
                        await UsersMovieListsService2.WatchlistService.RefreshWatchlist(1, 1000);
                    }
                    catch (Exception ex)
                    { await _pageService.DisplayAlert("Error", $"Could not refresh the watchlist, service responded with: {ex.Message}", "Ok"); }
                    IsRefreshing = false;
                }
                SelectedListType = 1;
            });

            FavoritesListViewSelectorCommand = new Command(async () =>
            {
                if (!(0 < UsersMovieListsService2.FavoriteMoviesListService.FavoriteMovies.MovieDetailModels.Count))
                {
                    IsRefreshing = true;
                    try
                    {
                        await UsersMovieListsService2.FavoriteMoviesListService.RefreshFavoriteMoviesList(1, 1000);
                    }
                    catch (Exception ex)
                    { await _pageService.DisplayAlert("Error", $"Could not refresh the favorites list, service responded with: {ex.Message}", "Ok"); }
                    IsRefreshing = false;
                }
                SelectedListType = 2;
            });

            CustomListsViewSelectorCommand = new Command(async () => 
            {
                if (UsersMovieListsService2.CustomListsService.UsersCustomLists.Count == 0)
                {
                    IsRefreshing = true;

                    try
                    {
                        await UsersMovieListsService2.CustomListsService.UpdateCustomLists(retryCount: 1, 1000, fromCache: false);

                        var selectedList = UsersMovieListsService2.CustomListsService.SelectedCustomList;

                        if (selectedList != null)
                            await UsersMovieListsService2.CustomListsService.UpdateSingleCustomList(selectedList.Id, retryCount: 1, 1000, fromCache: false);
                    }
                    catch (Exception ex)
                    { await _pageService.DisplayAlert("Error", $"Could not refresh custom lists, service responded with: {ex.Message}", "Ok"); }
                    IsRefreshing = false;
                }
                SelectedListType = 3;
            });

            RefreshCustomCommand = new Command(async () =>
            {
                IsRefreshing = true;
                try
                {
                    await UsersMovieListsService2.CustomListsService.UpdateCustomLists();
                }
                catch (Exception ex)
                {
                    await _pageService.DisplayAlert("Error", $"Could not update custom lists: {ex.Message}", "Ok");
                }

                IsRefreshing = false;
            });

            RefreshCustomListCommand = new Command(async () =>
            {
                if (UsersMovieListsService2.CustomListsService.SelectedCustomList != null)
                {
                    IsRefreshing = true;
                    try
                    {
                        await UsersMovieListsService2.CustomListsService.UpdateSingleCustomList(UsersMovieListsService2.CustomListsService.SelectedCustomList.Id);
                    }
                    catch (Exception ex)
                    {
                        await _pageService.DisplayAlert("Error", $"could not refresh list: {ex.Message}", "Ok");
                    }
                    IsRefreshing = false;
                }
            });

            RemoveCustomListCommand = new Command(async () =>
            {
                if (UsersMovieListsService2.CustomListsService.SelectedCustomList != null)
                {
                    if (UsersMovieListsService2.CustomListsService.SelectedCustomList.Movies?.Count > 0)
                    {
                        if (await _pageService.DisplayActionSheet("Delete nonempty list?", "Cancel", "Remove") != "Remove")
                            return;
                    }
                    try
                    {
                        await UsersMovieListsService2.CustomListsService.RemoveActiveCustomList();
                    }
                    catch (Exception ex)
                    {
                        await _pageService.DisplayAlert("Error", $"Could not remove active list: {ex.Message}", "Ok");
                    }
                }
            });

            AddCustomListCommand = new Command(async () => await _pageService.PushAsync(new AddListPageViewModel(this)));

            RemoveMovieFromCustomListCommand = new Command<MovieDetailModel>(async movie =>
            {
                try
                {
                    await UsersMovieListsService2.CustomListsService.RemoveMovieFromActiveList(movie.Id);
                }
                catch (Exception ex) { await _pageService.DisplayAlert("Error", $"Service responded with: {ex.Message}", "Ok"); }
            });

            RefreshFavoriteListCommand = new Command(async () => 
            {
                IsRefreshing = true;
                try
                {
                    await UsersMovieListsService2.FavoriteMoviesListService.RefreshFavoriteMoviesList();
                }
                catch (Exception ex)
                { await _pageService.DisplayAlert("Error", $"Could not refresh the favorites list, service responded with: {ex.Message}", "Ok"); }
                IsRefreshing = false;
            });

            LoadNextFavoritesPageCommand = new Command(async () =>
            {
                IsRefreshing = true;
                try
                {
                    await UsersMovieListsService2.FavoriteMoviesListService.TryLoadNextPage();
                }
                catch (Exception ex)
                { await _pageService.DisplayAlert("Error", $"Could not load favorites, service responded with: {ex.Message}", "Ok"); }
                IsRefreshing = false;
            });

            RefreshWatchlistCommand = new Command(async () =>
            {
                IsRefreshing = true;
                try
                {
                    await UsersMovieListsService2.WatchlistService.RefreshWatchlist();
                }
                catch (Exception ex)
                { await _pageService.DisplayAlert("Error", $"Could not refresh the watchlist, service responded with: {ex.Message}", "Ok"); }
                IsRefreshing = false;
            });

            LoadNextWatchlistPageCommand = new Command(async () =>
            {
                IsRefreshing = true;
                try
                {
                    await UsersMovieListsService2.WatchlistService.TryLoadNextPage();
                }
                catch (Exception ex)
                { await _pageService.DisplayAlert("Error", $"Could not load watchlist items, service responded with: {ex.Message}", "Ok"); }
                IsRefreshing = false;
            });
            OnItemTappedCommand = new Command<MovieDetailModel>(async movie => await _pageService.PushAsync(movie));
        }

        public async Task AddList(AddListPageViewModel addListPageViewModel)
        {
            try
            {
                await UsersMovieListsService2.CustomListsService.AddAndMakeActiveCustomList(addListPageViewModel.Name, addListPageViewModel.Description);
            }
            catch (Exception ex)
            {
                await _pageService.DisplayAlert("Error", $"Could not add list: {ex.Message}", "Ok");
            }
        }

        /// <summary>
        /// Initializes the default active movie list shown on the page.
        /// </summary>
        public async Task Initialize()
        {
            if (0 < UsersMovieListsService2.WatchlistService.Watchlist.MovieDetailModels.Count)
                return;

            try
            {
                await UsersMovieListsService2.WatchlistService.RefreshWatchlist();
                SelectedListType = 1;
            }
            catch (Exception ex)
            {
                await _pageService.DisplayAlert("Error", $"Could not load watchlist, service responded with {ex.Message}", "Ok");
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

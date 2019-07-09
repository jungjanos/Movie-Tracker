using System;
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

        #region CUSTOM_LIST_COMMANDS
        public Command RemoveCustomListCommand { get; private set; }
        public Command AddCustomListCommand { get; private set; }
        public Command RefreshCustomCommand { get; private set; }
        public Command RefreshCustomListCommand { get; private set; }        
        public Command RemoveMovieFromCustomListCommand { get; private set; }
        #endregion

        public Command MovieListEntryTappedCommand { get; private set; }


        public ListsPageViewModel3(
            UsersMovieListsService2 usersMovieListsService2,
            IPageService pageService)
        {
            _pageService = pageService;
            UsersMovieListsService2 = usersMovieListsService2;

            RefreshCustomCommand = new Command(async () =>
            {
                IsRefreshing = true;
                try
                {
                    await UsersMovieListsService2.UpdateCustomLists();
                }
                catch (Exception ex)
                {
                    await _pageService.DisplayAlert("Error", $"Could not update custom lists: {ex.Message}", "Ok");
                }
                
                IsRefreshing = false;
            });

            RefreshCustomListCommand = new Command(async () =>
            {                
                if (UsersMovieListsService2.SelectedCustomList != null)
                {
                    IsRefreshing = true;
                    try
                    {
                        await UsersMovieListsService2.UpdateSingleCustomList(UsersMovieListsService2.SelectedCustomList.Id);
                    }
                    catch (Exception ex)
                    {
                        await _pageService.DisplayAlert("Error", $"could not refresh list: {ex.Message}", "Ok");
                    }
                    IsRefreshing = false;
                }});

            RemoveCustomListCommand = new Command(async () =>
            {
                if (UsersMovieListsService2.SelectedCustomList != null)
                {
                    if(UsersMovieListsService2.SelectedCustomList.Movies?.Count > 0)
                    {
                        if (await _pageService.DisplayActionSheet("Delete nonempty list?", "Cancel", "Remove") != "Remove")
                            return;
                    }
                    try
                    {
                        await UsersMovieListsService2.RemoveActiveCustomList();
                    }
                    catch(Exception ex)
                    {
                        await _pageService.DisplayAlert("Error", $"Could not remove active list: {ex.Message}", "Ok");
                    }
                }
            });

            AddCustomListCommand = new Command(async () => await _pageService.PushAsync(new AddListPageViewModel(this)));

            RemoveMovieFromCustomListCommand = new Command<MovieDetailModel>(async movie => {
                try
                {
                    await UsersMovieListsService2.RemoveMovieFromActiveList(movie.Id);
                } catch (Exception ex) { await _pageService.DisplayAlert("Error", $"Service responded with: {ex.Message}", "Ok"); }
            });

            MovieListEntryTappedCommand = new Command<MovieDetailModel>(async movie => await _pageService.PushAsync(movie));
        }

        public async Task AddList(AddListPageViewModel addListPageViewModel)
        {
            try
            {
                await UsersMovieListsService2.AddAndMakeActiveCustomList(addListPageViewModel.Name, addListPageViewModel.Description);
            }
            catch (Exception ex)
            {
                await _pageService.DisplayAlert("Error", $"Could not add list: {ex.Message}", "Ok");
            }
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

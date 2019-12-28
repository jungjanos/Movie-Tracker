using Ch9.ApiClient;
using Ch9.Services.Contracts;

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Ch9.Services
{
    public class UsersMovieListsService2 : INotifyPropertyChanged
    {
        private readonly ISettings _settings;
        private readonly ITmdbCachedSearchClient _tmdbCachedSearchClient;
        private readonly ITmdbApiService _tmdbApiService;
        private readonly IMovieDetailModelConfigurator _movieDetailConfigurator;

        public CustomListsService CustomListsService { get; set; }
        public FavoriteMoviesListService FavoriteMoviesListService { get; set; }
        public WatchlistService WatchlistService { get; set; }


        public UsersMovieListsService2(
            ISettings settings,
            ITmdbCachedSearchClient tmdbCachedSearchClient,
            ITmdbApiService tmdbApiService,
            IMovieDetailModelConfigurator movieDetailConfigurator2)
        {
            _settings = settings;
            _tmdbCachedSearchClient = tmdbCachedSearchClient;
            _tmdbApiService = tmdbApiService;
            _movieDetailConfigurator = movieDetailConfigurator2;

            CustomListsService = new CustomListsService(_settings, _tmdbCachedSearchClient, _movieDetailConfigurator);
            FavoriteMoviesListService = new FavoriteMoviesListService(_settings, _tmdbApiService, _movieDetailConfigurator);
            WatchlistService = new WatchlistService(_settings, _tmdbCachedSearchClient, _movieDetailConfigurator);
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

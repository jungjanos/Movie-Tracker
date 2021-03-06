﻿using Ch9.Infrastructure.Extensions;
using Ch9.Services.Contracts;
using Ch9.Services.MovieListServices;
using Ch9.Models;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Ch9.Services
{
    public class WatchlistService : INotifyPropertyChanged
    {
        private readonly ISettings _settings;        
        private readonly ITmdbApiService _tmdbApiService;
        private readonly IMovieDetailModelConfigurator _movieDetailConfigurator;
        private readonly ICommand _sortOptionChangedCommand;

        private SearchResult _watchlist;

        public SearchResult Watchlist
        {
            get => _watchlist;
            set => SetProperty(ref _watchlist, value);
        }

        private string _sortBy;
        public string SortBy
        {
            get => _sortBy;
            set
            {
                if (SetProperty(ref _sortBy, value))
                    _sortOptionChangedCommand.Execute(null);
            }
        }

        public WatchlistService(ISettings settings,
            ITmdbApiService tmdbApiService,
            IMovieDetailModelConfigurator movieDetailConfigurator)
        {
            _settings = settings;            
            _tmdbApiService = tmdbApiService;
            _movieDetailConfigurator = movieDetailConfigurator;

            _sortBy = "created_at.desc";

            _watchlist = new SearchResult();
            _watchlist.InitializeOrClearMovieCollection();

            _sortOptionChangedCommand = new Command(async () =>
            {
                try
                {
                    await RefreshWatchlist(1, 1000);
                }
                catch { }
            });
        }

        /// <summary>
        /// Can throw. 
        /// First resets the client side collection of the watchlist to its initial state.
        /// Tries to reload the first page of the watchlist from the server.
        /// The Webrequest respects the ascending/descending property for the search. 
        /// </summary>        
        public async Task RefreshWatchlist(int retryCount = 0, int delayMilliseconds = 1000)
        {
            Watchlist.InitializeOrClearMovieCollection();

            if (!_settings.IsLoggedin)
                throw new Exception("Account error: user is not signed in");

            var response = await _tmdbApiService.TryGetMovieWatchlist(language: _settings.SearchLanguage, sortBy: SortBy, page: 1, retryCount: retryCount, delayMilliseconds: delayMilliseconds);

            if (!response.HttpStatusCode.IsSuccessCode())
                throw new Exception($"Could not refresh watchlist, TMDB server responded with {response.HttpStatusCode}");            

            Watchlist.AppendResult(response.MoviesOnWatchlist, _movieDetailConfigurator);
        }

        public async Task TryLoadNextPage(int retryCount = 0, int delayMilliseconds = 1000)
        {
            if (!_settings.IsLoggedin)
                throw new Exception("Account error: user is not signed in");

            if (!CanLoad)
                return;

            var response = await _tmdbApiService.TryGetMovieWatchlist(language: _settings.SearchLanguage, sortBy: SortBy, page: Watchlist.Page + 1, retryCount: retryCount, delayMilliseconds: delayMilliseconds);

            if (!response.HttpStatusCode.IsSuccessCode())
                throw new Exception($"Could not load watchlist items, TMDB server responded with {response.HttpStatusCode}");            

            Watchlist.AppendResult(response.MoviesOnWatchlist, _movieDetailConfigurator);
            OnPropertyChanged(nameof(CanLoad));
        }

        public bool CanLoad => Watchlist.Page == 0 ? false : Watchlist.Page >= Watchlist.TotalPages ? false : true;

        /// <summary>
        /// Can throw. 
        /// Tries to add or remove a movie to/from the watchlist depending on the desired state. 
        /// The current watchlist status of the movie both on the local list and on the server is not checked. 
        /// Trying to add (or remove) a movie which is (or is not) on the local or server's watchlist list does not throw. 
        /// </summary>        
        /// <param name="desiredState">true: tries to add, false: tries to remove</param>        
        public async Task ToggleWatchlistState(MovieDetailModel movie, bool desiredState, int retryCount = 1, int delayMilliseconds = 1000)
        {
            if (!_settings.IsLoggedin)
                throw new Exception("Account error: user is not signed in");

            if (movie == null)
                throw new Exception("Movie is invalid");

            var response = await _tmdbApiService.TryUpdateWatchlist("movie", desiredState, movie.Id, retryCount, delayMilliseconds);
            //

            if (response.HttpStatusCode.IsSuccessCode())
            {
                if (desiredState) // we added the movie to the server's watchlist
                {
                    if (!Watchlist.MovieDetailModels.Select(movie_ => movie_.Id).Contains(movie.Id))
                        InsertItem(movie);
                }
                else // we removed the movie from the server's watchlist
                {
                    var movieToRemove = Watchlist.MovieDetailModels.FirstOrDefault(movie_ => movie_.Id == movie.Id);
                    Watchlist.MovieDetailModels.Remove(movieToRemove);
                    Watchlist.TotalResults--;
                }
            }
            else
                throw new Exception($"Could not update watchlist state, server responded with: {response.HttpStatusCode}");
        }

        private void InsertItem(MovieDetailModel movie)
        {
            Watchlist.TotalResults++;
            if (SortBy == "created_at.desc")
                Watchlist.MovieDetailModels.Insert(0, movie);
            else if (SortBy == "created_at.asc")
            {
                if (!CanLoad)
                    Watchlist.MovieDetailModels.Add(movie);
            }
            else
                throw new ArgumentOutOfRangeException($"{nameof(InsertItem)}() encountered invalid value on {nameof(SortBy)} : \"{SortBy}\" was not accepted");
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

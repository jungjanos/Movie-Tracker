﻿using Ch9.ApiClient;
using Ch9.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Ch9.Utils
{
    public class WatchlistService : INotifyPropertyChanged
    {
        private readonly ISettings _settings;
        private readonly ITmdbCachedSearchClient _tmdbCachedSearchClient;
        private readonly IMovieDetailModelConfigurator _movieDetailConfigurator;

        private SearchResult _watchlist;
        public SearchResult Watchlist
        {
            get => _watchlist;
            set => SetProperty(ref _watchlist, value);
        }

        public string SortBy;

        public WatchlistService(ISettings settings,
            ITmdbCachedSearchClient tmdbCachedSearchClient,
            IMovieDetailModelConfigurator movieDetailConfigurator)
        {
            _settings = settings;
            _tmdbCachedSearchClient = tmdbCachedSearchClient;
            _movieDetailConfigurator = movieDetailConfigurator;

            SortBy = "created_at.desc";

            _watchlist = new SearchResult
            {
                MovieDetailModels = new ObservableCollection<MovieDetailModel>(),
                Page = 0,
                TotalPages = 0,
                TotalResults = 0
            };
        }

        private void ClearWatchlist()
        {
            Watchlist.MovieDetailModels.Clear();
            Watchlist.Page = 0;
            Watchlist.TotalPages = 0;
            Watchlist.TotalResults = 0;
        }

        //TODO : Refactor this into a public helper method (code dedup)
        /// <summary>
        /// Not allowed to throw. 
        /// Appends the movie collection in the "MovieDetailModels" property of the server response
        /// to the target's same named property. Updates the targets page and result counters from the serverResponse 
        /// </summary>
        /// <param name="targetList">this is the public observered collection wich is updated with data from the server's response</param>
        /// <param name="serverResponse">contains the server's response, containing movies to append to the observed collection and page/result counter</param>
        private void AppendResult(SearchResult targetList, SearchResult serverResponse)
        {
            if (serverResponse.MovieDetailModels.Count > 0)
            {
                _movieDetailConfigurator.SetImageSrc(serverResponse.MovieDetailModels);
                _movieDetailConfigurator.SetGenreNamesFromGenreIds(serverResponse.MovieDetailModels);

                foreach (MovieDetailModel movie in serverResponse.MovieDetailModels)
                    targetList.MovieDetailModels.Add(movie);

                targetList.Page = serverResponse.Page;
                targetList.TotalPages = serverResponse.TotalPages;
                targetList.TotalResults = serverResponse.TotalResults;
            }
        }

        /// <summary>
        /// Can throw. 
        /// First resets the client side collection of the watchlist to its initial state.
        /// Tries to reload the first page of the watchlist from the server.
        /// The Webrequest respects the ascending/descending property for the search. 
        /// </summary>        
        public async Task RefreshWatchlist(int retryCount = 0, int delayMilliseconds = 1000)
        {
            ClearWatchlist();

            if (!_settings.HasTmdbAccount)
                throw new Exception("Account error: user is not signed in");


            GetMovieWatchlistResult getWatchlist = await _tmdbCachedSearchClient.GetMovieWatchlist(sortBy: SortBy, page: 1, retryCount: retryCount, delayMilliseconds: delayMilliseconds);
            if (!getWatchlist.HttpStatusCode.IsSuccessCode())
                throw new Exception($"Could not refresh watchlist, TMDB server responded with {getWatchlist.HttpStatusCode}");

            SearchResult WatchlistFavoriteList = JsonConvert.DeserializeObject<SearchResult>(getWatchlist.Json);

            AppendResult(Watchlist, WatchlistFavoriteList);
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

﻿using Ch9.ApiClient;
using Ch9.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Ch9.Utils;

namespace Ch9.Test.TmdbNetworkClientTests
{
    // INTEGRATION TESTS
    // for the critical TmdbNetworkClient.UpdateFavoriteList(...) function accessing the TMDB WebAPI
    public class UpdateFavoriteListTests : IAsyncLifetime
    {
        private readonly ITestOutputHelper _output;
        Dictionary<string, object> _settingsKeyValues;
        Settings _settings;
        TmdbNetworkClient _client;

        int _movie1 = 297761;
        int _movie2 = 60800; // Macskafogó
        int _invalidMovieId = 99999999;

        public UpdateFavoriteListTests(ITestOutputHelper output)
        {
            _output = output;

            _settingsKeyValues = new Dictionary<string, object>();
            _settingsKeyValues[nameof(Settings.ApiKey)] = "764d596e888359d26c0dc49deffecbb3";
            _settingsKeyValues[nameof(Settings.SessionId)] = "563636d0e4a0b41b775ba7703cc5c985f36cffaf"; // !!!! correct it !!!!!
            _settings = new Settings(_settingsKeyValues);
            _client = new TmdbNetworkClient(_settings);
        }

        public Task DisposeAsync() => Task.CompletedTask;

        public async Task InitializeAsync()
        {
            GetFavoriteMoviesResult getFavoriteList = await _client.GetFavoriteMovies();
            SearchResult moviesOnFavoriteList = JsonConvert.DeserializeObject<SearchResult>(getFavoriteList.Json);

            foreach (var movie in moviesOnFavoriteList.MovieDetailModels)
                await _client.UpdateFavoriteList("movie", false, movie.Id);
        }

        [Fact]
        // happy path
        public async Task WhenAddingMovieNotOnFavoriteList_AddsMovie()
        {
            int movieToAdd = _movie1;
            UpdateFavoriteListResult response = await _client.UpdateFavoriteList("movie", true, movieToAdd);
            _output.WriteLine($"Server responded: {response.HttpStatusCode}");
            _output.WriteLine(response.Json);

            var getFavorites = await _client.GetFavoriteMovies();  

            Assert.True(response.HttpStatusCode.IsSuccessCode());
            Assert.Contains(movieToAdd.ToString(), getFavorites.Json);
        }

        [Fact]
        // failure path
        public async Task WhenAddingInvalidMovie_Returns404()
        {
            int movieToAdd = _invalidMovieId;
            UpdateFavoriteListResult response = await _client.UpdateFavoriteList("movie", true, movieToAdd);
            _output.WriteLine($"Server responded: {response.HttpStatusCode}");

            Assert.True(response.HttpStatusCode == System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        // failure path
        public async Task WhenRemovingInvalidMovie_Returns404()
        {
            int movieToRemove = _invalidMovieId;
            UpdateFavoriteListResult response = await _client.UpdateFavoriteList("movie", false, movieToRemove);
            _output.WriteLine($"Server responded: {response.HttpStatusCode}");

            Assert.True(response.HttpStatusCode == System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        // happy path
        public async Task WhenRemovingMovieOnFavoriteList_RemovesMovie()
        {
            // adding 
            await _client.UpdateFavoriteList(mediaType: "movie", add: true, mediaId: _movie1, accountId: null, retryCount: 0);
            await _client.UpdateFavoriteList(mediaType: "movie", add: true, mediaId: _movie2, accountId: null, retryCount: 0);

            // removing 
            var response = await _client.UpdateFavoriteList(mediaType: "movie", add: false, mediaId: _movie1, accountId: null, retryCount: 0);
            _output.WriteLine($"Server responded: {response.HttpStatusCode}");
            _output.WriteLine(response.Json);

            var favoriteListResponse = await _client.GetFavoriteMovies();

            Assert.DoesNotContain(_movie1.ToString(), favoriteListResponse.Json);
            Assert.Contains(_movie2.ToString(), favoriteListResponse.Json);
        }

        [Fact]
        // happy path
        public async Task WhenRemovingMovieNotOnFavoriteList_ReturnsSuccess()
        {
            // adding movie
            await _client.UpdateFavoriteList(mediaType: "movie", add: true, mediaId: _movie1, accountId: null, retryCount: 0);

            // removing other movie not on list
            var response = await _client.UpdateFavoriteList(mediaType: "movie", add: false, mediaId: _movie2, accountId: null, retryCount: 0);
            _output.WriteLine($"Server responded: {response.HttpStatusCode}");
            _output.WriteLine(response.Json);

            var favoriteListResponse = await _client.GetFavoriteMovies();

            Assert.DoesNotContain(_movie2.ToString(), favoriteListResponse.Json);
            Assert.True(response.HttpStatusCode.IsSuccessCode());
        }


        void PrintMovieList(SearchResult result)
        {
            _output.WriteLine($"Returned {result.MovieDetailModels.Count} results");
            _output.WriteLine($"page: {result.Page}");
            _output.WriteLine($"total pages: {result.TotalPages}");
            _output.WriteLine($"total results: {result.TotalResults}");

            if (result.MovieDetailModels.Count == 0)
                return;
            _output.WriteLine("=======MOVIES=======");

            foreach (MovieDetailModel movie in result.MovieDetailModels)
            {
                _output.WriteLine($"movie id: {movie.Id}");
                _output.WriteLine($"title: {movie.Title}");
                _output.WriteLine("----------------------");
            }
            _output.WriteLine("=====MOVIES END=====");
        }
    }
}

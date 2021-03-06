﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;
using Ch9.Models;
using Ch9.Services.LocalSettings;
using Ch9.Data.ApiClient;
using Ch9.Data.Contracts;

namespace Ch9.Test.TmdbNetworkClientTests
{
    // INTEGRATION TESTS
    // for the critical TmdbNetworkClient.GetMovieDetailsWithAccountStates(...) function accessing the TMDB WebAPI  
    // these tests only focus on the AccountMovieStates object part of the server's response!!!
    public class GetMovieDetailsWithAccountStatesTests_AccountMovieStates_ONLY : IAsyncLifetime
    {
        private readonly ITestOutputHelper _output;
        Dictionary<string, object> _settingsKeyValues;
        Settings _settings;
        TmdbNetworkClient _client;

        readonly int _movie = 60800; // Macskafogó
        readonly int _invalidMovie = 99999999;

        public GetMovieDetailsWithAccountStatesTests_AccountMovieStates_ONLY(ITestOutputHelper output)
        {
            _output = output;

            _settingsKeyValues = new Dictionary<string, object>();
            _settingsKeyValues[nameof(Settings.ApiKey)] = "764d596e888359d26c0dc49deffecbb3";
            _settingsKeyValues[nameof(Settings.SessionId)] = "563636d0e4a0b41b775ba7703cc5c985f36cffaf";
            _settings = new Settings(_settingsKeyValues, null);
            _client = new TmdbNetworkClient(null, _settings.ApiKey);
        }

        // cleans the states of the movie apriory
        public async Task InitializeAsync()
        {
            await _client.DeleteMovieRating(_settings.SessionId, _movie);
            await _client.UpdateFavoriteList(_settings.SessionId, "movie", false, _movie);
            await _client.UpdateWatchlist(_settings.SessionId, "movie", false, _movie);
            await Task.Delay(500);
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        // happy path
        public async Task WhenCalledOnValidMovie_ReturnsState()
        {
            bool isFavorite = false;
            bool onWatchlist = false;

            GetMovieDetailsWithAccountStatesResult response = await _client.GetMovieDetailsWithAccountStates(_settings.SessionId, id: _movie, retryCount: 0);
            _output.WriteLine($"TMDB server's response code {response.HttpStatusCode}");
            _output.WriteLine($"TMDB server's response message {response.Json}");

            AccountMovieStates states = response.ExtractAccountStates();
            PrintAccountMovieStates(states);

            Assert.True(response.HttpStatusCode == System.Net.HttpStatusCode.OK);
            Assert.True(states.IsFavorite == isFavorite);
            Assert.True(states.Rating == null);
            Assert.True(states.OnWatchlist == onWatchlist);
        }

        [Fact]
        // happy path
        public async Task WhenCalledOnValidFavoriteMovie_ReturnsState()
        {
            bool isFavorite = true;
            bool onWatchlist = false;

            await _client.UpdateFavoriteList(_settings.SessionId, "movie", add: true, _movie);

            GetMovieDetailsWithAccountStatesResult response = await _client.GetMovieDetailsWithAccountStates(_settings.SessionId, id: _movie, retryCount: 0);
            _output.WriteLine($"TMDB server's response code {response.HttpStatusCode}");
            _output.WriteLine($"TMDB server's response message {response.Json}");

            AccountMovieStates states = response.ExtractAccountStates();
            PrintAccountMovieStates(states);

            Assert.True(response.HttpStatusCode == System.Net.HttpStatusCode.OK);
            Assert.True(states.IsFavorite == isFavorite);
            Assert.True(states.Rating == null);
            Assert.True(states.OnWatchlist == onWatchlist);
        }

        [Fact]
        // happy path
        public async Task WhenCalledOnValidWatchlistMovie_ReturnsState()
        {
            bool isFavorite = false;
            bool onWatchlist = true;

            await _client.UpdateWatchlist(_settings.SessionId, "movie", add: true, _movie);

            GetMovieDetailsWithAccountStatesResult response = await _client.GetMovieDetailsWithAccountStates(_settings.SessionId, id: _movie, retryCount: 0);
            _output.WriteLine($"TMDB server's response code {response.HttpStatusCode}");
            _output.WriteLine($"TMDB server's response message {response.Json}");

            AccountMovieStates states = response.ExtractAccountStates();
            PrintAccountMovieStates(states);

            Assert.True(response.HttpStatusCode == System.Net.HttpStatusCode.OK);
            Assert.True(states.IsFavorite == isFavorite);
            Assert.True(states.Rating == null);
            Assert.True(states.OnWatchlist == onWatchlist);
        }

        [Fact]
        // happy path
        public async Task WhenCalledOnValidRatedMovie_ReturnsState()
        {
            bool isFavorite = false;
            bool onWatchlist = false;
            decimal rating = 9m;

            await _client.RateMovie(_settings.SessionId, rating, _movie);
            // Server side (TMDB WebAPI) needs this delay to propagate changes...
            await Task.Delay(2000);

            GetMovieDetailsWithAccountStatesResult response = await _client.GetMovieDetailsWithAccountStates(_settings.SessionId, id: _movie, retryCount: 0);
            _output.WriteLine($"TMDB server's response code {response.HttpStatusCode}");
            _output.WriteLine($"TMDB server's response message {response.Json}");

            AccountMovieStates states = response.ExtractAccountStates();
            PrintAccountMovieStates(states);

            Assert.True(response.HttpStatusCode == System.Net.HttpStatusCode.OK);
            Assert.True(states.IsFavorite == isFavorite);
            Assert.True(states.Rating.Value == rating);
            Assert.True(states.OnWatchlist == onWatchlist);
        }

        [Fact]
        // wierd path
        public async Task WhenCalledWithInvalidSessionId_ReturnsMovieDetailsAnd_200OK()
        {
            _settings.SessionId = "invalidSessionId";

            GetMovieDetailsWithAccountStatesResult response = await _client.GetMovieDetailsWithAccountStates(_settings.SessionId, id: _movie, retryCount: 0);
            _output.WriteLine($"TMDB server's response code {response.HttpStatusCode}");

            Assert.True(response.HttpStatusCode == System.Net.HttpStatusCode.OK);
            Assert.Null(response.ExtractAccountStates());
            _output.WriteLine($"Imdb id = {JsonConvert.DeserializeObject<MovieDetailModel>(response.Json).ImdbId}");
        }

        [Fact]
        // wierd path
        public async Task WhenCalledWithInvalidMovieId_Returns_404NotFound()
        {
            GetMovieDetailsWithAccountStatesResult response = await _client.GetMovieDetailsWithAccountStates(_settings.SessionId, id: _invalidMovie, retryCount: 0);
            _output.WriteLine($"TMDB server's response code {response.HttpStatusCode}");
            Assert.True(response.HttpStatusCode == System.Net.HttpStatusCode.NotFound);
            Assert.Null(response.ExtractAccountStates());
        }

        private void PrintAccountMovieStates(AccountMovieStates states)
        {
            _output.WriteLine("=========STATES==========");
            _output.WriteLine($"movie Id: {states.MovieId}");
            _output.WriteLine($"favorite: {states.IsFavorite}");
            _output.WriteLine($"rated: {states.Rating?.Value.ToString() ?? "not rated"}");
            _output.WriteLine($"on watchlist: {states.OnWatchlist}");

            _output.WriteLine("======= END STATES========");
        }
    }
}

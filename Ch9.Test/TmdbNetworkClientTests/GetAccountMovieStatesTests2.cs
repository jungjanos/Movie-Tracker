using Ch9.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Ch9.Services.LocalSettings;
using Ch9.Data.ApiClient;
using Ch9.Data.Contracts;

namespace Ch9.Test.TmdbNetworkClientTests
{
    // INTEGRATION TESTS
    // for the critical TmdbNetworkClient.GetAccountMovieStates(...) function accessing the TMDB WebAPI    
    public class GetAccountMovieStatesTests2 : IAsyncLifetime
    {
        private readonly ITestOutputHelper _output;
        readonly Dictionary<string, object> _settingsKeyValues;
        readonly Settings _settings;
        readonly TmdbNetworkClient _client;

        readonly int _movie = 60800; // Macskafogó
        readonly int _invalidMovie = 99999999;

        public GetAccountMovieStatesTests2(ITestOutputHelper output)
        {
            _output = output;
            _settingsKeyValues = new Dictionary<string, object>();
            _settingsKeyValues[nameof(Settings.ApiKey)] = "764d596e888359d26c0dc49deffecbb3";
            _settingsKeyValues[nameof(Settings.SessionId)] = "563636d0e4a0b41b775ba7703cc5c985f36cffaf"; // !!!! correct it !!!!!
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

            GetAccountMovieStatesResult response = await _client.GetAccountMovieStates(_settings.SessionId, mediaId: _movie, guestSessionId: null, retryCount: 0);
            _output.WriteLine($"TMDB server's response code {response.HttpStatusCode}");
            _output.WriteLine($"TMDB server's response message {response.Json}");

            AccountMovieStates states = response.DeserializeJsonIntoModel();
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

            GetAccountMovieStatesResult response = await _client.GetAccountMovieStates(_settings.SessionId, mediaId: _movie, guestSessionId: null, retryCount: 0);
            _output.WriteLine($"TMDB server's response code {response.HttpStatusCode}");
            _output.WriteLine($"TMDB server's response message {response.Json}");

            AccountMovieStates states = response.DeserializeJsonIntoModel();
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

            GetAccountMovieStatesResult response = await _client.GetAccountMovieStates(_settings.SessionId, mediaId: _movie, guestSessionId: null, retryCount: 0);
            _output.WriteLine($"TMDB server's response code {response.HttpStatusCode}");
            _output.WriteLine($"TMDB server's response message {response.Json}");

            AccountMovieStates states = response.DeserializeJsonIntoModel();
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

            GetAccountMovieStatesResult response = await _client.GetAccountMovieStates(_settings.SessionId, mediaId: _movie, guestSessionId: null, retryCount: 0);
            _output.WriteLine($"TMDB server's response code {response.HttpStatusCode}");
            _output.WriteLine($"TMDB server's response message {response.Json}");

            AccountMovieStates states = response.DeserializeJsonIntoModel();
            PrintAccountMovieStates(states);

            Assert.True(response.HttpStatusCode == System.Net.HttpStatusCode.OK);
            Assert.True(states.IsFavorite == isFavorite);
            Assert.True(states.Rating.Value == rating);
            Assert.True(states.OnWatchlist == onWatchlist);
        }


        [Fact]
        // failure path
        public async Task WhenCalledWithInvalidSessionId_Returns401Unauthorized()
        {
            _settings.SessionId = "invalidSessionId";

            GetAccountMovieStatesResult response = await _client.GetAccountMovieStates(_settings.SessionId, mediaId: _movie, guestSessionId: null, retryCount: 0);
            _output.WriteLine($"TMDB server's response code {response.HttpStatusCode}");

            Assert.True(response.HttpStatusCode == System.Net.HttpStatusCode.Unauthorized);
        }

        [Fact]
        // failure path
        public async Task WhenCalledWithInvalidMovieId_Returns404()
        {
            GetAccountMovieStatesResult response = await _client.GetAccountMovieStates(_settings.SessionId, mediaId: _invalidMovie, guestSessionId: null, retryCount: 0);
            _output.WriteLine($"TMDB server's response code {response.HttpStatusCode}");

            Assert.True(response.HttpStatusCode == System.Net.HttpStatusCode.NotFound);
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

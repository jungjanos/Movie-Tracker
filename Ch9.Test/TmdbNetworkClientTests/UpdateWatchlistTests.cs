using Ch9.ApiClient;
using Ch9.Services;
<<<<<<< HEAD
=======
using Ch9.Ui.Contracts.Models;
>>>>>>> Switch-to-Ui-Models
using Ch9.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Ch9.Test.TmdbNetworkClientTests
{
    // INTEGRATION TESTS
    // for the critical TmdbNetworkClient.UpdateWatchlist(...) function accessing the TMDB WebAPI
    public class UpdateWatchlistTests : IAsyncLifetime
    {
        private readonly ITestOutputHelper _output;
        readonly Dictionary<string, object> _settingsKeyValues;
        readonly Settings _settings;
        readonly TmdbNetworkClient _client;
        readonly int _movie1 = 297761;
        readonly int _movie2 = 60800; // Macskafogó
        readonly int _invalidMovieId = 99999999;

        public UpdateWatchlistTests(ITestOutputHelper output)
        {
            _output = output;

            _settingsKeyValues = new Dictionary<string, object>();
            _settingsKeyValues[nameof(Settings.ApiKey)] = "764d596e888359d26c0dc49deffecbb3";
            _settingsKeyValues[nameof(Settings.SessionId)] = "563636d0e4a0b41b775ba7703cc5c985f36cffaf"; // !!!! correct it !!!!!
            _settings = new Settings(_settingsKeyValues);
            _client = new TmdbNetworkClient(_settings, null);
        }

        // empty the watchlist if not yet empty
        public async Task InitializeAsync()
        {
            GetMovieWatchlistResult getWatchlist = await _client.GetMovieWatchlist();
            SearchResult moviesOnWatchlist = JsonConvert.DeserializeObject<SearchResult>(getWatchlist.Json);

            foreach (var movie in moviesOnWatchlist.MovieDetailModels)
                await _client.UpdateWatchlist("movie", false, movie.Id);
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        //happy path
        public async Task WhenAddingMovieNotOnWatchlist_AddsMovie()
        {
            var response = await _client.UpdateWatchlist(mediaType: "movie", add: true, mediaId: _movie1, accountId: null, retryCount: 0);
            _output.WriteLine($"Server responded: {response.HttpStatusCode}");
            _output.WriteLine(response.Json);

            var watchlistResponse = await _client.GetMovieWatchlist();

            Assert.Contains(_movie1.ToString(), watchlistResponse.Json);
        }

        [Fact]
        //happy path
        public async Task WhenAddingMovieAlreadyOnWatchlist_ReturnsSuccess()
        {
            await _client.UpdateWatchlist(mediaType: "movie", add: true, mediaId: _movie1, accountId: null, retryCount: 0);
            var response2 = await _client.UpdateWatchlist(mediaType: "movie", add: true, mediaId: _movie1, accountId: null, retryCount: 0);
            _output.WriteLine($"Server responded: {response2.HttpStatusCode}");
            _output.WriteLine(response2.Json);

            var watchlistResponse = await _client.GetMovieWatchlist();

            Assert.True(response2.HttpStatusCode.IsSuccessCode());
            Assert.Contains(_movie1.ToString(), watchlistResponse.Json);
        }

        [Fact]
        // failure path
        public async Task WhenAddingInvalidMovie_Returns404()
        {
            var response = await _client.UpdateWatchlist(mediaType: "movie", add: true, mediaId: _invalidMovieId, accountId: null, retryCount: 0);
            _output.WriteLine($"Server responded: {response.HttpStatusCode}");

            Assert.True(response.HttpStatusCode == System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        // failure path
        public async Task WhenRemovingInvalidMovie_Returns404()
        {
            var response = await _client.UpdateWatchlist(mediaType: "movie", add: false, mediaId: _invalidMovieId, accountId: null, retryCount: 0);
            _output.WriteLine($"Server responded: {response.HttpStatusCode}");

            Assert.True(response.HttpStatusCode == System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        // happy path
        public async Task WhenRemovingMovieOnWatchlist_RemovesMovie()
        {
            // adding 
            await _client.UpdateWatchlist(mediaType: "movie", add: true, mediaId: _movie1, accountId: null, retryCount: 0);
            await _client.UpdateWatchlist(mediaType: "movie", add: true, mediaId: _movie2, accountId: null, retryCount: 0);

            // removing 
            var response = await _client.UpdateWatchlist(mediaType: "movie", add: false, mediaId: _movie1, accountId: null, retryCount: 0);
            _output.WriteLine($"Server responded: {response.HttpStatusCode}");
            _output.WriteLine(response.Json);

            var watchlistResponse = await _client.GetMovieWatchlist();

            Assert.DoesNotContain(_movie1.ToString(), watchlistResponse.Json);
            Assert.Contains(_movie2.ToString(), watchlistResponse.Json);
        }

        [Fact]
        // happy path
        public async Task WhenRemovingMovieNotOnWatchlist_ReturnsSuccess()
        {
            // adding movie
            await _client.UpdateWatchlist(mediaType: "movie", add: true, mediaId: _movie1, accountId: null, retryCount: 0);

            // removing other movie not on list
            var response = await _client.UpdateWatchlist(mediaType: "movie", add: false, mediaId: _movie2, accountId: null, retryCount: 0);
            _output.WriteLine($"Server responded: {response.HttpStatusCode}");
            _output.WriteLine(response.Json);

            var watchlistResponse = await _client.GetMovieWatchlist();

            Assert.DoesNotContain(_movie2.ToString(), watchlistResponse.Json);
            Assert.True(response.HttpStatusCode.IsSuccessCode());
        }
    }
}

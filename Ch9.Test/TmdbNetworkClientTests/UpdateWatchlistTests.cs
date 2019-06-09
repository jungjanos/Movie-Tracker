using Ch9.ApiClient;
using Ch9.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Ch9.Test.TmdbNetworkClientTests
{
    // INTEGRATION TESTS
    // for the critical TmdbNetworkClient.UpdateWatchlist(...) function accessing the TMDB WebAPI
    public class UpdateWatchlistTests
    {
        private readonly ITestOutputHelper _output;
        Dictionary<string, object> _settingsKeyValues;
        Settings _settings;
        TmdbNetworkClient _client;

        int _movie1 = 297761;
        //int _movie2 = 60800; // Macskafogó
        //int _invalidMovieId = 99999999;

        public UpdateWatchlistTests(ITestOutputHelper output)
        {
            _output = output;

            _settingsKeyValues = new Dictionary<string, object>();
            _settingsKeyValues[nameof(Settings.ApiKey)] = "764d596e888359d26c0dc49deffecbb3";
            _settingsKeyValues[nameof(Settings.SessionId)] = "563636d0e4a0b41b775ba7703cc5c985f36cffaf"; // !!!! correct it !!!!!
            _settings = new Settings(_settingsKeyValues);
            _client = new TmdbNetworkClient(_settings);
        }

        [Fact]
        //happy path
        public async Task WhenAddingMovieNotOnWatchlist_AddsMovie()
        {
            var response = await  _client.UpdateWatchlist(mediaType: "movie", add: true, mediaId: _movie1, accountId: null, retryCount: 0);
            _output.WriteLine($"Server responded: {response.HttpStatusCode}");
            _output.WriteLine(response.Json);

            var watchlistResponse = await _client.GetMovieWatchlist();

             Assert.Contains(_movie1.ToString(), watchlistResponse.Json);
        }
        [Fact]
        public async Task WhenRemovinggMovieNotOnWatchlist_RemovesMovie()
        {
            // adding 
            await _client.UpdateWatchlist(mediaType: "movie", add: true, mediaId: _movie1, accountId: null, retryCount: 0);

            // removing
            var response = await _client.UpdateWatchlist(mediaType: "movie", add: false, mediaId: _movie1, accountId: null, retryCount: 0);
            _output.WriteLine($"Server responded: {response.HttpStatusCode}");
            _output.WriteLine(response.Json);

            var watchlistResponse = await _client.GetMovieWatchlist();

            Assert.DoesNotContain(_movie1.ToString(), watchlistResponse.Json);
        }

    }
}

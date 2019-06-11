using Ch9.ApiClient;
using Ch9.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Ch9.Test.TmdbNetworkClientTests
{
    // INTEGRATION TESTS
    // for the critical TmdbNetworkClient.GetMovieWatchlist(...) function accessing the TMDB WebAPI
    public class GetMovieWatchlistTests : IAsyncLifetime
    {
        private readonly ITestOutputHelper _output;
        readonly Dictionary<string, object> _settingsKeyValues;
        readonly Settings _settings;
        readonly TmdbNetworkClient _client;
        readonly int _movie1 = 297761;
        readonly int _movie2 = 60800; // Macskafogó

        public GetMovieWatchlistTests(ITestOutputHelper output)
        {
            _output = output;

            _settingsKeyValues = new Dictionary<string, object>();
            _settingsKeyValues[nameof(Settings.ApiKey)] = "764d596e888359d26c0dc49deffecbb3";
            _settingsKeyValues[nameof(Settings.SessionId)] = "563636d0e4a0b41b775ba7703cc5c985f36cffaf"; // !!!! correct it !!!!!
            _settings = new Settings(_settingsKeyValues);
            _client = new TmdbNetworkClient(_settings);
        }

        // empty the watchlist if not yet empty
        public async Task InitializeAsync()
        {
            GetMovieWatchlistResult getWatchlist = await _client.GetMovieWatchlist();
            SearchResult moviesOnWatchlist = JsonConvert.DeserializeObject<SearchResult>(getWatchlist.Json);

            foreach(var movie in moviesOnWatchlist.MovieDetailModels)
                await _client.UpdateWatchlist("movie", false, movie.Id);
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        // happy path
        public async Task WhenCalledOnEmptyList_ReturnsEmptyCollection()
        {

            GetMovieWatchlistResult result = await _client.GetMovieWatchlist(accountId: null, language: null, sortBy: null, page: null, retryCount: 0);
            _output.WriteLine($"Server returned {result.HttpStatusCode}");
            _output.WriteLine($"Json: {result.Json}");

            var watchlist = JsonConvert.DeserializeObject<SearchResult>(result.Json);

            PrintWatchlist(watchlist);

            Assert.True(watchlist.MovieDetailModels.Count == 0);
        }


        [Fact]
        // happy path
        public async Task WhenCalledOnNonemptyList_GivesBackList()
        {
            await _client.UpdateWatchlist("movie", true, _movie1);
            await _client.UpdateWatchlist("movie", true, _movie2);

            GetMovieWatchlistResult result = await _client.GetMovieWatchlist(accountId: null, language: null, sortBy: null, page: null, retryCount: 0);
            _output.WriteLine($"Server returned {result.HttpStatusCode}");
            _output.WriteLine($"Json: {result.Json}");

            var watchlist = JsonConvert.DeserializeObject<SearchResult>(result.Json);

            PrintWatchlist(watchlist);

            Assert.True(watchlist.MovieDetailModels.Count > 0);
            Assert.Contains(watchlist.MovieDetailModels, movie => movie.Id == _movie1);
            Assert.Contains(watchlist.MovieDetailModels, movie => movie.Id == _movie2);
        }

        [Fact]
        // happy path
        public async Task WhenCalledWithSortOption_RespectsSortRequest()
        {
            await _client.UpdateWatchlist("movie", true, _movie1);
            await _client.UpdateWatchlist("movie", true, _movie2);


            GetMovieWatchlistResult resultAsc = await _client.GetMovieWatchlist(accountId: null, language: null, sortBy: "created_at.asc", page: null, retryCount: 0);
            var watchlistAsc = JsonConvert.DeserializeObject<SearchResult>(resultAsc.Json);
            var movieIdsAsc = watchlistAsc.MovieDetailModels.Select(movie => movie.Id);

            GetMovieWatchlistResult resultDesc = await _client.GetMovieWatchlist(accountId: null, language: null, sortBy: "created_at.desc", page: null, retryCount: 0);
            var watchlistDesc = JsonConvert.DeserializeObject<SearchResult>(resultDesc.Json);            
            var movieIdsDesc = watchlistDesc.MovieDetailModels.Select(movie => movie.Id);

            PrintWatchlist(watchlistAsc);
            PrintWatchlist(watchlistDesc);

            Assert.True(movieIdsAsc.Reverse().SequenceEqual(movieIdsDesc));
        }

        [Fact]
        // failure path
        public async Task WhenCalledWithInvalidSessionId_ReturnsUnauthorized401()
        {
            _settings.SessionId = "invalidSessionId";

            GetMovieWatchlistResult result = await _client.GetMovieWatchlist(accountId: null, language: null, sortBy: null, page: null, retryCount: 0);
            _output.WriteLine($"Server returned {result.HttpStatusCode}");

            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.Unauthorized);
        }

        void PrintWatchlist(SearchResult result)
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

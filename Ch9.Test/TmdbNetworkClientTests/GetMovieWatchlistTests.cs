using Ch9.ApiClient;
using Ch9.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Ch9.Test.TmdbNetworkClientTests
{
    // INTEGRATION TESTS
    // for the critical TmdbNetworkClient.GetMovieWatchlist(...) function accessing the TMDB WebAPI
    public class GetMovieWatchlistTests
    {
        private readonly ITestOutputHelper _output;
        Dictionary<string, object> _settingsKeyValues;
        Settings _settings;
        TmdbNetworkClient _client;

        public GetMovieWatchlistTests(ITestOutputHelper output)
        {
            _output = output;

            _settingsKeyValues = new Dictionary<string, object>();
            _settingsKeyValues[nameof(Settings.ApiKey)] = "764d596e888359d26c0dc49deffecbb3";
            _settingsKeyValues[nameof(Settings.SessionId)] = "563636d0e4a0b41b775ba7703cc5c985f36cffaf"; // !!!! correct it !!!!!
            _settings = new Settings(_settingsKeyValues);
            _client = new TmdbNetworkClient(_settings);
        }

        [Fact]
        // happy path
        public async Task WhenCalledOnNonemptyList_GivesBackList()
        {
            GetMovieWatchlistResult result = await _client.GetMovieWatchlist(accountId: null, language: null, sortBy: null, page: null, retryCount: 0);
            _output.WriteLine($"Server returned {result.HttpStatusCode}");
            _output.WriteLine($"Json: {result.Json}");

            var watchlist = JsonConvert.DeserializeObject<SearchResult>(result.Json);

            PrintWatchlist(watchlist);

            Assert.True(watchlist.MovieDetailModels.Count > 0);
        }

        [Fact]
        // happy path
        public async Task WhenCalledWithSortOption_RespectsSortRequest()
        {
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

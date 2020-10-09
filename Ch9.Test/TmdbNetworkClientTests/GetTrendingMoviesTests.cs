using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;
using Ch9.Models;
using Ch9.Infrastructure.Extensions;
using Ch9.Services.LocalSettings;
using Ch9.Data.ApiClient;

namespace Ch9.Test.TmdbNetworkClientTests
{
    // INTEGRATION TESTS
    // for the critical TmdbNetworkClient.GetTrendingMovies(...) function accessing the TMDB WebAPI
    public class GetTrendingMoviesTests
    {
        private readonly ITestOutputHelper _output;
        Dictionary<string, object> _settingsKeyValues;
        Settings _settings;
        TmdbNetworkClient _client;

        public GetTrendingMoviesTests(ITestOutputHelper output)
        {
            _output = output;

            _settingsKeyValues = new Dictionary<string, object>();
            _settingsKeyValues[nameof(Settings.ApiKey)] = "764d596e888359d26c0dc49deffecbb3";
            _settings = new Settings(_settingsKeyValues, null);
            _client = new TmdbNetworkClient(null, _settings.ApiKey);
        }

        [Fact]
        // happy path
        public async Task CalledForWeekPeriod_ReturnsFirstPageOfListOfMovies()
        {
            var result = await _client.GetTrendingMovies(week: true, language: null, includeAdult: null, page: null, retryCount: 0, delayMilliseconds: 1000);
            _output.WriteLine(result.Json);

            var movies = JsonConvert.DeserializeObject<SearchResult>(result.Json);

            Assert.True(result.HttpStatusCode.IsSuccessCode());
            Assert.True(movies.MovieDetailModels.Count > 0);
            Assert.True(movies.Page == 1);
        }

        [Fact]
        // happy path
        public async Task CalledFoDayPeriod_ReturnsFirstPageOfListOfMovies()
        {
            var result = await _client.GetTrendingMovies(week: false, language: null, includeAdult: null, page: null, retryCount: 0, delayMilliseconds: 1000);
            _output.WriteLine(result.Json);

            var movies = JsonConvert.DeserializeObject<SearchResult>(result.Json);

            Assert.True(result.HttpStatusCode.IsSuccessCode());
            Assert.True(movies.MovieDetailModels.Count > 0);
            Assert.True(movies.Page == 1);
        }

        [Fact]
        // happy path
        public async Task CalledWithLanguageOption_RespectsLanguageParameter()
        {
            var result = await _client.GetTrendingMovies(week: true, language: "de", includeAdult: null, page: null, retryCount: 0, delayMilliseconds: 1000);
            _output.WriteLine(result.Json);

            // German pronouns
            Assert.Contains("der", result.Json);
            Assert.Contains("die", result.Json);
            Assert.Contains("das", result.Json);
        }

        [Fact]
        //happy path
        public async Task CalledWithAdultOption_RespectsAdultParameter()
        {
            var result = await _client.GetTrendingMovies(week: true, language: null, includeAdult: true, page: null, retryCount: 0, delayMilliseconds: 1000);
            _output.WriteLine(result.Json);

            var movies = JsonConvert.DeserializeObject<SearchResult>(result.Json);

            Assert.True(result.HttpStatusCode.IsSuccessCode());
            Assert.True(movies.MovieDetailModels.Count > 0);
            Assert.True(movies.Page == 1);
        }

        [Fact]
        //happy path
        public async Task CalledWithPageParameter_RespectsQueriedPage()
        {
            var result_page_4 = await _client.GetTrendingMovies(week: true, language: null, includeAdult: true, page: 4, retryCount: 0, delayMilliseconds: 1000);
            _output.WriteLine(result_page_4.Json);

            var result_page_5 = await _client.GetTrendingMovies(week: true, language: null, includeAdult: true, page: 5, retryCount: 0, delayMilliseconds: 1000);
            _output.WriteLine(result_page_5.Json);

            var movies_page_4 = JsonConvert.DeserializeObject<SearchResult>(result_page_4.Json);
            var movies_page_5 = JsonConvert.DeserializeObject<SearchResult>(result_page_5.Json);

            Assert.True(movies_page_4.MovieDetailModels.Count > 0 && movies_page_5.MovieDetailModels.Count > 0);
            Assert.True(movies_page_4.Page == 4);
            Assert.True(movies_page_5.Page == 5);
        }
    }
}

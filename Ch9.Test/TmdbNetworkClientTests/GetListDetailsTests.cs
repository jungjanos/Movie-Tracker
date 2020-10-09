using System.Collections.Generic;
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
    public class GetListDetailsTests
    {
        private readonly ITestOutputHelper _output;
        readonly Dictionary<string, object> _settingsKeyValues;
        readonly Settings _settings;
        readonly TmdbNetworkClient _client;

        public GetListDetailsTests(ITestOutputHelper output)
        {
            _output = output;
            _settingsKeyValues = new Dictionary<string, object>();
            _settingsKeyValues[nameof(Settings.ApiKey)] = "764d596e888359d26c0dc49deffecbb3";
            _settingsKeyValues[nameof(Settings.SessionId)] = "563636d0e4a0b41b775ba7703cc5c985f36cffaf"; // !!!! correct it !!!!!
            _settings = new Settings(_settingsKeyValues, null);
            _client = new TmdbNetworkClient(null, _settings.ApiKey);
        }

        [Fact]
        // happy path
        public async Task WhenCalledWithValidId_ReturnsListDetails()
        {
            // Arrange 
            int listId = 109137; // valid id

            // Act
            var result = await _client.GetListDetails(_settings.SessionId, listId);
            PrintTrace(listId, result);

            var movieListResponse = JsonConvert.DeserializeObject<MovieListModel>(result?.Json);
            _output.WriteLine($"List contains {movieListResponse.Movies.Count} movies");

            // Assert
            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.OK);
            Assert.True(movieListResponse.Movies != null);
        }

        [Fact]
        // happy path
        public async Task WhenQueriesEmptyList_ReturnsListDetailsWithEmptyCollection()
        {
            // Arrange 
            int listId = 109867; // valid id of an empty list

            // Act
            var result = await _client.GetListDetails(_settings.SessionId, listId);
            PrintTrace(listId, result);

            var movieListResponse = JsonConvert.DeserializeObject<MovieListModel>(result?.Json);
            _output.WriteLine($"List contains {movieListResponse.Movies.Count} movies");

            // Assert
            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.OK);
            Assert.True(movieListResponse.Movies.Count == 0);
        }

        [Fact]
        // failure path
        public async Task WhenCalledWithInvalidId_Returns404()
        {
            // Arrange 
            int listId = 1231527006; // invalid id

            // Act
            var result = await _client.GetListDetails(_settings.SessionId, listId);
            PrintTrace(listId, result);

            // Assert
            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.NotFound);
        }

        private void PrintTrace(int listId, GetListDetailsResult result)
        {
            _output.WriteLine($"{nameof(_client.GetListDetails)}({listId}) called");
            _output.WriteLine($"TMDB server responded: {result.HttpStatusCode}");
            if (result.Json != null)
                _output.WriteLine($"response: \n {result.Json}");
        }
    }
}

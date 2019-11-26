using Ch9.ApiClient;
using Ch9.Services;
using Ch9.Ui.Contracts.Models;
using Ch9.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Ch9.Test.TmdbNetworkClientTests
{
    // INTEGRATION TESTS    
    // While the code here works as expected, targeted TMDB WebAPI function is EXTREMELY FRAGILE (breaks often bc of the movie lists 'description' field) !!!
    // DO NOT RUN TESTS IN PARALLEL
    // for the critical TmdbNetworkClient.RemoveMovie(...) function accessing the TMDB WebAPI
    public class RemoveMovieTests : IAsyncLifetime
    {
        private int _listId;
        private List<int> _validMovieIds;

        private readonly ITestOutputHelper _output;
        Dictionary<string, object> _settingsKeyValues;
        Settings _settings;
        TmdbNetworkClient _client;
        public RemoveMovieTests(ITestOutputHelper output)
        {
            _output = output;

            _settingsKeyValues = new Dictionary<string, object>();
            _settingsKeyValues[nameof(Settings.ApiKey)] = "764d596e888359d26c0dc49deffecbb3";
            _settingsKeyValues[nameof(Settings.SessionId)] = "563636d0e4a0b41b775ba7703cc5c985f36cffaf"; // !!!! correct it !!!!!
            _settings = new Settings(_settingsKeyValues);
            _client = new TmdbNetworkClient(_settings, null);

            _validMovieIds = new List<int>
            {
                23047, // Season of the Witch
                1542 // Office Space
            };
        }

        // Setup: create a temporary list with movies for the tests
        public async Task InitializeAsync()
        {
            var createListResult = await _client.CreateList("Test list1 with movies", "test list for movie deletion");
            _output.WriteLine($"{nameof(InitializeAsync)}: {nameof(_client.CreateList)}() returned {createListResult.HttpStatusCode}");
            _listId = JsonConvert.DeserializeObject<ListCrudResponseModel>(createListResult.Json).ListId;
            _output.WriteLine($"{nameof(InitializeAsync)}: list created with id {_listId}");

            foreach (var mediaId in _validMovieIds)
            {
                var result = await _client.AddMovie(_listId, mediaId);
                _output.WriteLine($"{nameof(InitializeAsync)}: {nameof(_client.AddMovie)}(list: {_listId}, mediaId: {mediaId}) responded with: {result.HttpStatusCode}");
            }

        }
        // Teardown: remove the list created by the Setup code
        public async Task DisposeAsync()
        {
            var result = await _client.DeleteList(_listId);
            // The TMDB WebAPI has some glitch with the CreateList response code: Http.500 == Success !!
            _output.WriteLine($"{nameof(DisposeAsync)}: {nameof(_client.DeleteList)}({_listId}) returned {(result.HttpStatusCode == System.Net.HttpStatusCode.InternalServerError ? "Success" : "some failure...")}");
        }
        [Fact]
        // happy path
        public async Task WhenMediaIdIsValid_RemovesMedia()
        {
            // Arrange
            var mediaId = _validMovieIds.First();

            // Act
            var result = await _client.RemoveMovie(_listId, mediaId);
            _output.WriteLine($"{nameof(_client.RemoveMovie)}(list: {_listId}, mediaId: {mediaId}) responded with: {result.HttpStatusCode}");
            if (result.HttpStatusCode.IsSuccessCode())
                _output.WriteLine($"TMDB server's response message {result.Json}");

            // Assert
            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.OK);

            var listDetailsResult = await _client.GetListDetails(_listId);
            var remainingMovieCount = JsonConvert.DeserializeObject<MovieListModel>(listDetailsResult.Json).Movies.Count;

            // Assert
            Assert.True(remainingMovieCount == _validMovieIds.Count - 1);
        }

        [Fact]
        // happy path
        // NOT INTUITIVE: Responds with Http.200 instead of Http.404
        public async Task WhenListDoesNotContainRequestedMedia_RespondsWith200()
        {
            // Arrange
            var mediaId = 29383; // Valid media id, but our list does not contain it

            // Act
            var result = await _client.RemoveMovie(_listId, mediaId);
            _output.WriteLine($"{nameof(_client.RemoveMovie)}(list: {_listId}, mediaId: {mediaId}) responded with: {result.HttpStatusCode}");
            if (result.HttpStatusCode.IsSuccessCode())
                _output.WriteLine($"TMDB server's response message {result.Json}");

            // Assert
            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.OK);

            var listDetailsResult = await _client.GetListDetails(_listId);
            var remainingMovieCount = JsonConvert.DeserializeObject<MovieListModel>(listDetailsResult.Json).Movies.Count;

            Assert.True(remainingMovieCount == _validMovieIds.Count);
        }

        [Fact]
        // failure path
        public async Task WhenSessionIdIsInvalid_RespondsWith401()
        {
            // Arrange
            var mediaId = _validMovieIds.First();
            var temp = _settings.SessionId;
            _settings.SessionId = "someinvalidsessionId";

            // Act
            var result = await _client.RemoveMovie(_listId, mediaId);
            _settings.SessionId = temp;
            _output.WriteLine($"{nameof(_client.RemoveMovie)}(list: {_listId}, mediaId: {mediaId}) responded with: {result.HttpStatusCode}");

            // Assert
            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.Unauthorized);

            var listDetailsResult = await _client.GetListDetails(_listId);
            var remainingMovieCount = JsonConvert.DeserializeObject<MovieListModel>(listDetailsResult.Json).Movies.Count;

            // Assert
            Assert.True(remainingMovieCount == _validMovieIds.Count);
        }
    }
}

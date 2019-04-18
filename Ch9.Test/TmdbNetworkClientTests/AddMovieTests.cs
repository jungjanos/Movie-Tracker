using System.Collections.Generic;
using Ch9.Models;
using Ch9.ApiClient;
using Xunit;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Xunit.Abstractions;
using System.Linq;
using Ch9.Utils;

namespace Ch9.Test.TmdbNetworkClientTests
{
    // INTEGRATION TESTS
    // While the code here works as expected, targeted TMDB WebAPI function is EXTREMELY FRAGILE!!!
    // DO NOT RUN TESTS IN PARALLEL, IT BREAKS THE EXPECTED SERVER BEVAVIOR
    // for the critical TmdbNetworkClient.AddMovie(...) function accessing the TMDB WebAPI
    public class AddMovieTests : IAsyncLifetime
    {
        private int _listId;
        private List<int> _validMovieIds; 

        private readonly ITestOutputHelper _output;
        Dictionary<string, object> _settingsKeyValues;
        Settings _settings;
        TmdbNetworkClient _client;
        public AddMovieTests(ITestOutputHelper output)
        {
            _output = output;

            _settingsKeyValues = new Dictionary<string, object>();
            _settingsKeyValues[nameof(Settings.ApiKey)] = "764d596e888359d26c0dc49deffecbb3";
            _settingsKeyValues[nameof(Settings.SessionId)] = "563636d0e4a0b41b775ba7703cc5c985f36cffaf"; // !!!! correct it !!!!!
            _settings = new Settings(_settingsKeyValues);
            _client = new TmdbNetworkClient(_settings);

            _validMovieIds = new List<int>
            {
                23047, // Season of the Witch
                1542 // Office Space
            };
        }

        // Setup: create a temporary list for the tests
        public async Task InitializeAsync()
        {
            var createListResult = await _client.CreateList("Test list 2", "some test list tralalala la í ő ű z ");
            _output.WriteLine($"{nameof(InitializeAsync)}: {nameof(_client.CreateList)} returned {createListResult.HttpStatusCode}");
            _listId = JsonConvert.DeserializeObject<ListCrudResponseModel>(createListResult.Json).ListId;
            _output.WriteLine($"{nameof(InitializeAsync)}: list created with id {_listId}");
        }

        // Teardown
        public async Task DisposeAsync()
        {
            var result = await _client.DeleteList(_listId);
            // The TMDB WebAPI has some glitch with the CreateList response code: Http.500 == Success !!
            _output.WriteLine($"{nameof(DisposeAsync)}: {nameof(_client.DeleteList)}({_listId}) returned {(result.HttpStatusCode == System.Net.HttpStatusCode.InternalServerError ? "Success" : "some failure...")}");
        }

        [Fact]
        // happy path
        public async Task WhenValidMovieId_AddsMovie()
        {
            // Arrange
            int mediaId = _validMovieIds.First();

            // Act
            var result = await _client.AddMovie(_listId, mediaId);
            _output.WriteLine($"{nameof(_client.AddMovie)}(list: {_listId}, mediaId: {mediaId}) responded with: {result.HttpStatusCode}");

            if (result.HttpStatusCode.IsSuccessCode())
                _output.WriteLine($"TMDB server's response message {result.Json}");

            // Assert
            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.Created);
        }

        [Fact]
        // happy path
        public async Task WhenValidMovieIds_AddsMovies()
        {
            // Act
            foreach(var mediaId in _validMovieIds)
            {
                var result = await _client.AddMovie(_listId, mediaId);
                _output.WriteLine($"{nameof(_client.AddMovie)}(list: {_listId}, mediaId: {mediaId}) responded with: {result.HttpStatusCode}");
                if (result.HttpStatusCode.IsSuccessCode())
                    _output.WriteLine($"TMDB server's response message {result.Json}");
            }

            var listDetailResult = await _client.GetListDetails(_listId);
            MovieListModel listDetails = null;
            if (listDetailResult.HttpStatusCode.IsSuccessCode())
            {
                 listDetails = JsonConvert.DeserializeObject<MovieListModel>(listDetailResult.Json);
                _output.WriteLine($"After update list contains {listDetails.Movies.Length}");
            }

             // Assert
            Assert.True(listDetails.Movies.Length == _validMovieIds.Count);
        }

        [Fact]
        // failure path
        public async Task WhenInvalidMediaId_Returns404()
        {
            // Arrange
            int mediaId = 12345; // invalid media id

            // Act
            var result = await _client.AddMovie(_listId, mediaId);
            _output.WriteLine($"{nameof(_client.AddMovie)}(list: {_listId}, mediaId: {mediaId}) responded with: {result.HttpStatusCode}");

            // Assert
            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        // failure path
        public async Task WhenInvalidSessionId_Returns401()
        {
            // Arrange
            var temp = _settings.SessionId;
            var mediaId = _validMovieIds.First();
            _settings.SessionId = "thisisaninvalidsessionid";

            // Act
            var result = await _client.AddMovie(_listId, mediaId);
            _settings.SessionId = temp;
            _output.WriteLine($"{nameof(_client.AddMovie)}(list: {_listId}, mediaId: {mediaId}) responded with: {result.HttpStatusCode}");

            // Assert
            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.Unauthorized);
        }
    }
}

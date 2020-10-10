using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;
using Ch9.Models;
using Ch9.Services.LocalSettings;
using Ch9.Data.ApiClient;

namespace Ch9.Test.TmdbNetworkClientTests
{
    // INTEGRATION TESTS
    // for the important TmdbNetworkClient.DeleteList(...) function accessing the TMDB WebAPI
    public class DeleteListTests
    {
        private readonly ITestOutputHelper _output;
        Dictionary<string, object> _settingsKeyValues;
        Settings _settings;
        TmdbNetworkClient _client;
        public DeleteListTests(ITestOutputHelper output)
        {
            _output = output;

            _settingsKeyValues = new Dictionary<string, object>();
            _settingsKeyValues[nameof(Settings.ApiKey)] = "764d596e888359d26c0dc49deffecbb3";
            _settingsKeyValues[nameof(Settings.SessionId)] = "17e9c7d453286dbd089842c056f5316605516f26"; 
            _settings = new Settings(_settingsKeyValues, null);
            _client = new TmdbNetworkClient(null, _settings.ApiKey);
        }

        [Fact]
        // happy path
        // NOT INTUITIVE: glitch on server side, HTTP response code for success is 500 (Internal Server Error)
        public async Task WhenListExists_Deletes()
        {
            // Arrange
            string name = "New list to delete";
            string description = "delete test";
            string language = "en";

            var createListResult = await _client.CreateList(_settings.SessionId, name, description, language);
            var createResponse = JsonConvert.DeserializeObject<ListCrudResponseModel>(createListResult?.Json);

            int? id = createResponse?.ListId;

            _output.WriteLine($"{nameof(_client.CreateList)}({name}, .., ..) called");
            _output.WriteLine($"TMDB server responded: {createListResult.HttpStatusCode}");
            _output.WriteLine($"with id: {id}");

            // Act
            var deleteResult = await _client.DeleteList(_settings.SessionId, id.Value);

            _output.WriteLine($"Calling {nameof(_client.DeleteList)}({id})");
            _output.WriteLine($"TMDB server responded: {deleteResult.HttpStatusCode}");

            // Assert
            Assert.True(deleteResult.HttpStatusCode == System.Net.HttpStatusCode.InternalServerError);
        }

        [Fact]
        // failure path        
        public async Task WhenListDoesNotExists_ReturnsNotFound404()
        {
            // Arrange some invalid Id:
            int id = 1234024256;

            // Act
            var deleteResult = await _client.DeleteList(_settings.SessionId, id);

            _output.WriteLine($"Calling {nameof(_client.DeleteList)}({id})");
            _output.WriteLine($"TMDB server responded: {deleteResult.HttpStatusCode}");

            // Assert
            Assert.True(deleteResult.HttpStatusCode == System.Net.HttpStatusCode.NotFound);
        }
    }
}

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
    // DO NOT RUN TESTS IN PARALLEL
    // for the critical TmdbNetworkClient.CreateList(...) function accessing the TMDB WebAPI
    public class CreateListTests : IAsyncLifetime
    {
        private readonly ITestOutputHelper _output;
        Dictionary<string, object> _settingsKeyValues;
        Settings _settings;
        TmdbNetworkClient _client;

        List<int> _listIdsToDispose;

        public CreateListTests(ITestOutputHelper output)
        {
            _output = output;
            _listIdsToDispose = new List<int>();
            _settingsKeyValues = new Dictionary<string, object>();
            _settingsKeyValues[nameof(Settings.ApiKey)] = "764d596e888359d26c0dc49deffecbb3";
            _settingsKeyValues[nameof(Settings.SessionId)] = "17e9c7d453286dbd089842c056f5316605516f26";
            _settings = new Settings(_settingsKeyValues, null);
            _client = new TmdbNetworkClient(null, _settings.ApiKey);
        }

        public async Task DisposeAsync()
        {
            foreach (int id in _listIdsToDispose)
                await _client.DeleteList(_settings.SessionId, id);
        }

        public Task InitializeAsync() => Task.CompletedTask;

        [Fact]
        // happy path
        public async Task WhenArgumentsValid_CreatesNewList()
        {
            // Arrange
            string name = "New list";
            string description = "test";
            string language = "en";

            // Act
            var result = await _client.CreateList(_settings.SessionId, name, description, language);
            _output.WriteLine($"TMDB server responded: {result.HttpStatusCode.ToString()}");

            if (result.HttpStatusCode == System.Net.HttpStatusCode.Created)
                _listIdsToDispose.Add(JsonConvert.DeserializeObject<ListCrudResponseModel>(result.Json).ListId);

            // Assert
            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.Created);
        }

        [Fact]
        // happy path
        // REMARK: The created lists have the same name, but unique Id as primary key
        public async Task WhenListAlreadyExists_CreatesNewListWithSameName()
        {
            // Arrange
            string name = "New list double";
            string description = "test double";
            string language = "en";

            var result1 = await _client.CreateList(_settings.SessionId, name, description, language);
            if (result1.HttpStatusCode == System.Net.HttpStatusCode.Created)
                _listIdsToDispose.Add(JsonConvert.DeserializeObject<ListCrudResponseModel>(result1.Json).ListId);

            // Act
            var result2 = await _client.CreateList(_settings.SessionId, name, description, language);
            _output.WriteLine($"TMDB server responded: {result2.HttpStatusCode.ToString()}");
            if (result2.HttpStatusCode == System.Net.HttpStatusCode.Created)
                _listIdsToDispose.Add(JsonConvert.DeserializeObject<ListCrudResponseModel>(result2.Json).ListId);

            // Assert
            Assert.True(result2.HttpStatusCode == System.Net.HttpStatusCode.Created);
        }

        [Fact]
        // failure path        
        public async Task WhenCalledWithInvalidSessionId_RespondsWithErrorCode401()
        {
            // Arrange
            string name = "New list 3";
            string description = "description";
            string language = "en";
            var temp = _settings.SessionId;
            _settings.SessionId = "thisisaninvalidsessionid";

            // Act
            var result = await _client.CreateList(_settings.SessionId, name, description, language);
            _settings.SessionId = temp;

            _output.WriteLine($"TMDB server responded: {result.HttpStatusCode.ToString()}");

            // Assert
            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.Unauthorized);
        }

        [Fact]
        // failure path        
        public async Task WhenCalledWithInvalidCharacters_RespondsWithErrorCode422()
        {
            // Arrange
            string name = "New list ( )  ";
            string description = "description";
            string language = "en";

            await _client.CreateList(name, description, language);

            // Act
            var result = await _client.CreateList(_settings.SessionId, name, description, language);
            _output.WriteLine($"TMDB server responded: {result.HttpStatusCode.ToString()}");

            // Assert
            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.UnprocessableEntity);
        }
    }
}

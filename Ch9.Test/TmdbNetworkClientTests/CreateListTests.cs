﻿using Ch9.ApiClient;
using Ch9.Services;
using Ch9.Ui.Contracts.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

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
            _settingsKeyValues[nameof(Settings.SessionId)] = "563636d0e4a0b41b775ba7703cc5c985f36cffaf"; // !!!! correct it !!!!!
            _settings = new Settings(_settingsKeyValues);
            _client = new TmdbNetworkClient(_settings, null);
        }

        public async Task DisposeAsync()
        {
            foreach (int id in _listIdsToDispose)
                await _client.DeleteList(id);
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        [Fact]
        // happy path
        public async Task WhenArgumentsValid_CreatesNewList()
        {
            // Arrange
            string name = "New list";
            string description = "test";
            string language = "en";

            // Act
            var result = await _client.CreateList(name, description, language);
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

            var result1 = await _client.CreateList(name, description, language);
            if (result1.HttpStatusCode == System.Net.HttpStatusCode.Created)
                _listIdsToDispose.Add(JsonConvert.DeserializeObject<ListCrudResponseModel>(result1.Json).ListId);

            // Act
            var result2 = await _client.CreateList(name, description, language);
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
            var result = await _client.CreateList(name, description, language);
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
            var result = await _client.CreateList(name, description, language);
            _output.WriteLine($"TMDB server responded: {result.HttpStatusCode.ToString()}");

            // Assert
            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.UnprocessableEntity);
        }


    }
}

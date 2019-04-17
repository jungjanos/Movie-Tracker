﻿using System.Collections.Generic;
using Ch9.Models;
using Ch9.ApiClient;
using Xunit;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Ch9.Test.TmdbNetworkClientTests
{
    public class CreateListTests : IAsyncLifetime
    {
        private readonly ITestOutputHelper _output;
        Dictionary<string, object> _settingsKeyValues;
        Settings _settings;
        TmdbNetworkClient _client;

        List<int> listIdsToDispose;

        public CreateListTests(ITestOutputHelper output)
        {
            _output = output;
            listIdsToDispose = new List<int>();
            _settingsKeyValues = new Dictionary<string, object>();
            _settingsKeyValues[nameof(Settings.ApiKey)] = "764d596e888359d26c0dc49deffecbb3";
            _settingsKeyValues[nameof(Settings.SessionId)] = "563636d0e4a0b41b775ba7703cc5c985f36cffaf"; // !!!! correct it !!!!!
            _settings = new Settings(_settingsKeyValues);
            _client = new TmdbNetworkClient(_settings);
        }

        public async Task DisposeAsync()
        {
            foreach (int id in listIdsToDispose)
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
                listIdsToDispose.Add(JsonConvert.DeserializeObject<ListCrudResponseModel>(result.Json).ListId);

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
                listIdsToDispose.Add(JsonConvert.DeserializeObject<ListCrudResponseModel>(result1.Json).ListId);



            // Act
            var result2 = await _client.CreateList(name, description, language);
            _output.WriteLine($"TMDB server responded: {result2.HttpStatusCode.ToString()}");
            if (result2.HttpStatusCode == System.Net.HttpStatusCode.Created)
                listIdsToDispose.Add(JsonConvert.DeserializeObject<ListCrudResponseModel>(result2.Json).ListId);

            // Assert
            Assert.True(result2.HttpStatusCode == System.Net.HttpStatusCode.Created);
        }

        [Fact]
        // failure path        
        public async Task WhenListCalledWithInvalidSessionId_RespondsWithErrorCode401()
        {
            // Arrange
            string name = "New list 3";
            string description = "description";
            string language = "en";
            _settings.SessionId = "thisisaninvalidsessionid";

            await _client.CreateList(name, description, language);

            // Act
            var result = await _client.CreateList(name, description, language);
            _output.WriteLine($"TMDB server responded: {result.HttpStatusCode.ToString()}");

            // Assert
            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.Unauthorized);
        }


    }
}
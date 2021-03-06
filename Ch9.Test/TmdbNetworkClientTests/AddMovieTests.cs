﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;
using Ch9.Models;
using Ch9.Data.ApiClient;
using Ch9.Services.LocalSettings;
using Ch9.Infrastructure.Extensions;

namespace Ch9.Test.TmdbNetworkClientTests
{
    // INTEGRATION TESTS
    // While the code here works as expected, targeted TMDB WebAPI function is EXTREMELY FRAGILE!!!
    // DO NOT RUN TESTS IN PARALLEL, IT BREAKS THE EXPECTED SERVER BEHAVIOR
    // for the critical TmdbNetworkClient.AddMovie(...) function accessing the TMDB WebAPI
    public class AddMovieTests : IAsyncLifetime
    {
        private int _listId;
        private readonly List<int> _validMovieIds;

        private readonly ITestOutputHelper _output;
        readonly Dictionary<string, object> _settingsKeyValues;
        readonly Settings _settings;
        readonly TmdbNetworkClient _client;
        public AddMovieTests(ITestOutputHelper output)
        {
            _output = output;

            _settingsKeyValues = new Dictionary<string, object>();
            _settingsKeyValues[nameof(Settings.ApiKey)] = "764d596e888359d26c0dc49deffecbb3";
            _settingsKeyValues[nameof(Settings.SessionId)] = "17e9c7d453286dbd089842c056f5316605516f26"; 
            _settings = new Settings(_settingsKeyValues, null);
            _client = new TmdbNetworkClient(null, _settings.ApiKey);

            _validMovieIds = new List<int>
            {
                23047, // Season of the Witch
                1542 // Office Space
            };
        }

        // Setup: create a temporary list for the tests
        public async Task InitializeAsync()
        {
            var createListResult = await _client.CreateList(sessionId: _settings.SessionId, name: "Test list X", description: ""); // 2020.10.10 for some reason API is picky when accepting description parameter
            _output.WriteLine($"{nameof(InitializeAsync)}: {nameof(_client.CreateList)} returned {createListResult.HttpStatusCode}");
            _listId = JsonConvert.DeserializeObject<ListCrudResponseModel>(createListResult.Json).ListId;
            _output.WriteLine($"{nameof(InitializeAsync)}: list created with id {_listId}");
        }

        // Teardown
        public async Task DisposeAsync()
        {
            var result = await _client.DeleteList(_settings.SessionId, _listId);
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
            var result = await _client.AddMovie(_settings.SessionId, _listId, mediaId);
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
            foreach (var mediaId in _validMovieIds)
            {
                var result = await _client.AddMovie(_settings.SessionId, _listId, mediaId);
                _output.WriteLine($"{nameof(_client.AddMovie)}(list: {_listId}, mediaId: {mediaId}) responded with: {result.HttpStatusCode}");
                if (result.HttpStatusCode.IsSuccessCode())
                    _output.WriteLine($"TMDB server's response message {result.Json}");
            }

            var listDetailResult = await _client.GetListDetails(_settings.SessionId, _listId);
            MovieListModel listDetails = null;
            if (listDetailResult.HttpStatusCode.IsSuccessCode())
            {
                listDetails = JsonConvert.DeserializeObject<MovieListModel>(listDetailResult.Json);
                _output.WriteLine($"After update list contains {listDetails.Movies.Count}");
            }

            // Assert
            Assert.True(listDetails.Movies.Count == _validMovieIds.Count);
        }

        [Fact]
        // failure path
        public async Task WhenInvalidMediaId_Returns404()
        {
            // Arrange
            int mediaId = 12345; // invalid media id

            // Act
            var result = await _client.AddMovie(_settings.SessionId, _listId, mediaId);
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
            var result = await _client.AddMovie(_settings.SessionId, _listId, mediaId);
            _settings.SessionId = temp;
            _output.WriteLine($"{nameof(_client.AddMovie)}(list: {_listId}, mediaId: {mediaId}) responded with: {result.HttpStatusCode}");

            // Assert
            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.Unauthorized);
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;
using Ch9.Models;
using Ch9.Services.LocalSettings;
using Ch9.Data.ApiClient;
using Ch9.Data.Contracts;

// INTEGRATION TESTS
// for the critical TmdbNetworkClient.GetItemStatusOnTargetList(...) function accessing the TMDB WebAPI
namespace Ch9.Test.TmdbNetworkClientTests
{
    public class GetItemStatusOnTargetListTests
    {
        private readonly ITestOutputHelper _output;
        Dictionary<string, object> _settingsKeyValues;
        Settings _settings;
        TmdbNetworkClient _client;

        private int _movieId1 = 550;
        private int _listId1 = 10482;
        private int _listIdNotContainingMovieId1 = 9660;

        private int _invalidList = 99999999;

        public GetItemStatusOnTargetListTests(ITestOutputHelper output)
        {
            _output = output;

            _settingsKeyValues = new Dictionary<string, object>();
            _settingsKeyValues[nameof(Settings.ApiKey)] = "764d596e888359d26c0dc49deffecbb3";
            _settings = new Settings(_settingsKeyValues, null);
            _client = new TmdbNetworkClient(null, _settings.ApiKey);
        }

        [Fact]
        // happy path
        public async Task CalledWithMovieOnList_ReturnsSuccess()
        {
            GetItemStatusOnTargetListResult result = await _client.GetItemStatusOnTargetList(listId: _listId1, movieId: _movieId1);
            _output.WriteLine($"Server responded with {result.HttpStatusCode}");

            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.OK);
        }

        [Fact]
        // happy path
        public async Task CalledWithMovieOnList_ReturnsTrueAsResult()
        {
            GetItemStatusOnTargetListResult result = await _client.GetItemStatusOnTargetList(listId: _listId1, movieId: _movieId1);
            _output.WriteLine($"Json: {result.Json}");

            ItemStatusOnTargetList statusObject = JsonConvert.DeserializeObject<ItemStatusOnTargetList>(result.Json);

            Assert.True(statusObject.ItemPresent);
        }

        [Fact]
        // happy path
        public async Task CalledWithMovieNotOnList_ReturnsFalseAsResult()
        {
            GetItemStatusOnTargetListResult result = await _client.GetItemStatusOnTargetList(listId: _listIdNotContainingMovieId1, movieId: _movieId1);
            _output.WriteLine($"Json: {result.Json}");

            ItemStatusOnTargetList statusObject = JsonConvert.DeserializeObject<ItemStatusOnTargetList>(result.Json);

            Assert.False(statusObject.ItemPresent);
        }

        [Fact]
        // awkward path
        public async Task CalledOnInvalidList_ReturnsOk200()
        {
            GetItemStatusOnTargetListResult result = await _client.GetItemStatusOnTargetList(listId: _invalidList, movieId: _movieId1);
            _output.WriteLine($"Json: {result.HttpStatusCode}");
            _output.WriteLine($"Json: {result.Json}");

            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.OK);
        }

        [Fact]
        // awkward path
        public async Task CalledOnInvalidList_ReturnsFalseAsResult()
        {
            GetItemStatusOnTargetListResult result = await _client.GetItemStatusOnTargetList(listId: _invalidList, movieId: _movieId1);
            _output.WriteLine($"Json: {result.HttpStatusCode}");
            _output.WriteLine($"Json: {result.Json}");

            ItemStatusOnTargetList statusObject = JsonConvert.DeserializeObject<ItemStatusOnTargetList>(result.Json);

            Assert.False(statusObject.ItemPresent);
        }

        [Theory]
        [InlineData(1000)]
        [InlineData(2000)]
        [InlineData(3000)]
        // awkward path
        public async Task CalledWithIntegerListId_AlwaysReturnsNullAsIdField(int _list)
        {
            GetItemStatusOnTargetListResult result = await _client.GetItemStatusOnTargetList(listId: _list, movieId: _movieId1);
            _output.WriteLine($"Json: {result.HttpStatusCode}");
            _output.WriteLine($"Json: {result.Json}");

            ItemStatusOnTargetList statusObject = JsonConvert.DeserializeObject<ItemStatusOnTargetList>(result.Json);

            Assert.Null(statusObject.Id);
        }
    }
}

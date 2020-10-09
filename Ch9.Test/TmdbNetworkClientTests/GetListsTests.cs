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
    // for the critical TmdbNetworkClient.GetLists(...) function accessing the TMDB WebAPI
    public class GetListsTests
    {
        private readonly ITestOutputHelper _output;
        Dictionary<string, object> _settingsKeyValues;
        Settings _settings;
        TmdbNetworkClient _client;

        public GetListsTests(ITestOutputHelper output)
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
        public async Task AccountIdMissing_ReturnsListForCurrentAccount()
        {
            // Act
            var result = await _client.GetLists(_settings.SessionId);
            _output.WriteLine($"Server returned {result.HttpStatusCode}, message: {result?.Json ?? "some error..."}");
            GetListsModel lists = JsonConvert.DeserializeObject<GetListsModel>(result?.Json);

            // Assert
            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.OK);
            Assert.True(result?.Json != null);
            Assert.True(lists != null);
        }

        [Fact]
        // happy path
        public async Task CalledWithUsersOwnAccountId_ReturnsListForCurrentAccount()
        {
            // Act
            var result = await _client.GetLists(_settings.SessionId, 8341984);
            _output.WriteLine($"Server returned {result.HttpStatusCode}, message: {result?.Json ?? "some error..."}");
            GetListsModel lists = JsonConvert.DeserializeObject<GetListsModel>(result?.Json);

            // Assert
            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.OK);
            Assert.True(result?.Json != null);
            Assert.True(lists != null);
        }

        [Fact]
        // happy path
        // NOT INTUITIVE
        public async Task CalledWithInvalidAccountId_ReturnsListForCurrentAccount()
        {
            // Arrange
            int invalidId = 6666666;

            // Act
            var result = await _client.GetLists(_settings.SessionId, invalidId);
            _output.WriteLine($"Server returned {result.HttpStatusCode}, message: {result?.Json ?? "some error..."}");
            GetListsModel lists = JsonConvert.DeserializeObject<GetListsModel>(result?.Json);

            // Assert
            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.OK);
            Assert.True(result?.Json != null);
            Assert.True(lists != null);
        }

        [Theory]
        // happy path        
        [InlineData("en")]
        [InlineData("hu")]
        [InlineData("de")]
        public async Task CalledWithLanguageOption_DoesNotBreak(string language)
        {
            // Act
            var result = await _client.GetLists(_settings.SessionId, language: language);
            _output.WriteLine($"Server returned {result.HttpStatusCode}, message: {result?.Json ?? "some error..."}");
            GetListsModel lists = JsonConvert.DeserializeObject<GetListsModel>(result?.Json);

            // Assert
            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.OK);
            Assert.True(result?.Json != null);
            Assert.True(lists != null);
        }

        [Theory]
        // happy path  
        [InlineData(null)]
        [InlineData(1)]
        // NOT INTUITIVE: TMDB WebAPI does not respect page query: always sends back the first page
        // when only one page exists even if the second page was asked for!!!
        [InlineData(2)]
        public async Task CalledWithPageOptions_DoesNotBreakWhenPageDoesNotexist(int? page)
        {
            // Act
            var result = await _client.GetLists(_settings.SessionId, page: page);
            _output.WriteLine($"Server returned {result.HttpStatusCode}, message: {result?.Json ?? "some error..."}");
            GetListsModel lists = JsonConvert.DeserializeObject<GetListsModel>(result?.Json);

            // Assert
            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.OK);
            Assert.True(result?.Json != null);
            Assert.True(lists != null);
        }

        [Fact]
        // error path        
        public async Task CalledWithInvalidSessionId_ReturnsUnathorized()
        {
            // Arrange
            string invalidSessionId = "someinvalidsessionid";
            _settings.SessionId = invalidSessionId;

            // Act
            var result = await _client.GetLists(_settings.SessionId);
            _output.WriteLine($"Server returned {result.HttpStatusCode}, message: {result?.Json ?? "some error..."}");

            // Assert
            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.Unauthorized);
        }
    }
}

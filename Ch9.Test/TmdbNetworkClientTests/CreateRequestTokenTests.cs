using System.Collections.Generic;
using Ch9.Authentication;
using Ch9.Models;
using Ch9.ApiClient;
using Xunit;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Ch9.Test.TmdbNetworkClientTests
{
    // INTEGRATION TESTS

    // for the critical TmdbNetworkClient.CreateRequesttoken(...) function accessing the TMDB WebAPI
    // No intention to achieve full coverage for all paths:
    // WebAPI Http 404 error conditions is unclear (when should we receive code 404 ? )
    public class CreateRequestTokenTests
    {
        Dictionary<string, object> _settingsKeyValues;        
        Settings _settings;
        TmdbNetworkClient _client;

        public CreateRequestTokenTests()
        {            
            _settingsKeyValues = new Dictionary<string, object>();
            _settingsKeyValues[nameof(Settings.ApiKey)] = "764d596e888359d26c0dc49deffecbb3";
            _settings = new Settings(_settingsKeyValues);
            _client = new TmdbNetworkClient(_settings);
            Trace.WriteLine(nameof(CreateRequestTokenTests) + " constructor passed");
        }

        [Fact]
        // Happy path /wo retry option
        public async void WhenApiKeyIsValid_CreatesRequestToken()
        {                        

            //Act
            CreateRequestTokenResult result = await _client.CreateRequestToken();
            RequestToken token = JsonConvert.DeserializeObject<RequestToken>(result.Json);

            //Assert
            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.OK);
            Assert.True(token.Success);
            Assert.False(string.IsNullOrWhiteSpace(token.Token));
        }
        [Fact]
        // Happy path with retry option set 
        // (will only retry when code is manually set for that)
        public async void WhenApiKeyIsValid_AndWithRetryOptionSet_CreatesRequestToken()
        {
            //Act
            CreateRequestTokenResult result = await _client.CreateRequestToken(3, 1000);
            RequestToken token = JsonConvert.DeserializeObject<RequestToken>(result.Json);

            //Assert
            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.OK);
            Assert.True(token.Success);
            Assert.False(string.IsNullOrWhiteSpace(token.Token));
        }

        [Fact]
        public async void WhenApiKeyIsInvalid_DoesNotBreakAndGivesErrorCode()
        {
            //Arrange            
            _settingsKeyValues[nameof(Settings.ApiKey)] = "invalidkeytest1012";            

            //Act
            CreateRequestTokenResult result = await _client.CreateRequestToken();           

            //Assert
            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.Unauthorized);            
        }

        [Fact]
        public async void WhenApiKeyIsInvalidAndCalledMultipleTimes_DoesNotBreakAndGivesErrorCode()
        {
            //Arrange
            _settingsKeyValues[nameof(Settings.ApiKey)] = "invalidkeytest1012";

            //Act
            CreateRequestTokenResult result = await _client.CreateRequestToken(3, 1000);

            //Assert
            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.Unauthorized);
        }
    }
}

using System.Collections.Generic;
using Ch9.Authentication;
using Ch9.Models;
using Ch9.ApiClient;
using Xunit;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Xunit.Abstractions;
using System;

namespace Ch9.Test.TmdbNetworkClientTests
{
    // INTEGRATION TESTS
    // for the critical TmdbNetworkClient.DeleteSession(...) function accessing the TMDB WebAPI
    public class DeleteSessionTests : IAsyncLifetime
    {
        private string SessionId { get; set; }

        private readonly ITestOutputHelper _output;
        Dictionary<string, object> _settingsKeyValues;
        Settings _settings;
        TmdbNetworkClient _client;

        // Setup steps:
        // -(1) set up a new unused request token
        // -(2) Validate the request token
        // -(3) Create a new session

        public DeleteSessionTests(ITestOutputHelper output)
        {
            _output = output;

            _settingsKeyValues = new Dictionary<string, object>();
            _settingsKeyValues[nameof(Settings.ApiKey)] = "764d596e888359d26c0dc49deffecbb3";
            _settingsKeyValues[nameof(Settings.AccountName)] = "j4nitest";
            _settingsKeyValues[nameof(Settings.Password)] = "awx123.";
            _settings = new Settings(_settingsKeyValues);
            _client = new TmdbNetworkClient(_settings, null);            
        }

        public async Task InitializeAsync()
        {
            try
            {
                var tokenResponse = await _client.CreateRequestToken();
                var token = JsonConvert.DeserializeObject<RequestToken>(tokenResponse.Json);
                await _client.ValidateRequestTokenWithLogin
                    (_settings.AccountName, _settings.Password, token.Token);

                var createSessionIdResult = await _client.CreateSessionId(token.Token);
                SessionId = JsonConvert.DeserializeObject<SessionIdResponseModel>(createSessionIdResult.Json).SessionId;
                _output.WriteLine($"{nameof(InitializeAsync)}: Session \"{SessionId}\" created");
            }
            catch (Exception ex)
            {
                _output.WriteLine(ex.Message);
                _output.WriteLine(ex.StackTrace);
            }            
        }

        public async Task DisposeAsync()
        {
            _output.WriteLine($"{nameof(DisposeAsync)}: DeleteSession({SessionId}) called...");
            var result = await _client.DeleteSession(SessionId);
            var code = result.HttpStatusCode.ToString();
            _output.WriteLine($"{nameof(DisposeAsync)}: DeleteSession(...) returned with {code}");
        }

        [Theory]
        [InlineData(0, 1000)]
        [InlineData(3, 1000)]
        public async Task WhenSessionIsValid_DeletesSession(int retryCount, int delayMilliseconds)
        {
            // Act
            var result = await _client.DeleteSession(SessionId, retryCount, delayMilliseconds );

            // Assert
            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.OK);
            _output.WriteLine($"Server returned {result.Json}");
        }

        [Fact]
        // failure path
        // RESULT IS NOT INTUITIVE!!!
        public async Task WhenSessionIsInvalid_Returns200Code()
        {
            // Act
            var result = await _client.DeleteSession("thisisaninvalidsessionid");            
            _output.WriteLine($"Server returned {result.HttpStatusCode}, {result.Json}");
            // Assert
            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.OK);   
        }
    }
}

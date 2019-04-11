using System.Collections.Generic;
using Ch9.Authentication;
using Ch9.Models;
using Ch9.ApiClient;
using Xunit;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Ch9.Test.TmdbNetworkClientTests
{
    // INTEGRATION TESTS

    public class DeleteSessionTests
    {
        private readonly ITestOutputHelper _output;
        Dictionary<string, object> _settingsKeyValues;
        Settings _settings;
        TmdbNetworkClient _client;
        Task<RequestToken> _getToken;
        Task<Task<RequestToken>> _validateToken;
        Task<Task<SessionIdResponseModel>> _getSession;

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
            _client = new TmdbNetworkClient(_settings);

            Task<CreateRequestTokenResult> result = _client.CreateRequestToken();

            _getToken = result.ContinueWith(t =>
            {
                var json = t.Result;
                RequestToken token = JsonConvert.DeserializeObject<RequestToken>(json.Json);
                return token;
            });

            _validateToken = _getToken.ContinueWith(async t =>
            {
                var token = t.Result.Token;

                var validationResponse = await _client.ValidateRequestTokenWithLogin(
                           _settings.AccountName,
                           _settings.Password,
                           token);

                return JsonConvert.DeserializeObject<RequestToken>(validationResponse.Json);
            });

            _getSession = _validateToken.ContinueWith(async t =>
            {
                var token = t.Result.Result.Token;

                var createSessionIdResult = await _client.CreateSessionId(token);
                return JsonConvert.DeserializeObject<SessionIdResponseModel>(createSessionIdResult.Json);
            });
        }

        [Theory]
        [InlineData(0, 1000)]
        [InlineData(3, 1000)]
        public async void WhenSessionIsValid_DeletesSession(int retryCount, int delayMilliseconds)
        {
            // Arrange
            string requestToken = (await await _validateToken).Token;
            _output.WriteLine($"Request token {requestToken} successfully created and validated");

            string sessionId = (await await _getSession).SessionId;
            if (!(await await _getSession).Success) throw new System.Exception($"Error at arranging test, {nameof(TmdbNetworkClient.CreateSessionId)} failed");
            _output.WriteLine($"Session id {sessionId} successfully created and validated");

            // Act
            var result = await _client.DeleteSession(sessionId, retryCount, delayMilliseconds );

            // Assert
            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.OK);
            _output.WriteLine($"Server returned {result.Json}");
        }

        [Fact]
        // failure path
        // RESULT IS NOT INTUITIVE!!!
        public async void WhenSessionIsInValid_Returns200Code()
        {
            // Teardown (was created in the ctor needs to be torn down)
            await await _validateToken;
            string sessionId = (await await _getSession).SessionId;
            var result2 = await _client.DeleteSession(sessionId);
            if (result2.HttpStatusCode == System.Net.HttpStatusCode.OK)
                _output.WriteLine($"Teardown successfull");
            else
            {
                _output.WriteLine($"Teardown failure, error code {result2.HttpStatusCode}");
                _output.WriteLine(result2?.Json);
            }

            // Act
            var result = await _client.DeleteSession("thisisaninvalidsessionid");            
            _output.WriteLine($"Server returned {result.HttpStatusCode}, {result.Json}");
            // Assert
            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.OK);   
        }


    }
}

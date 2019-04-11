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
    public class CreateSessionIdTests
    {
        private readonly ITestOutputHelper _output;
        Dictionary<string, object> _settingsKeyValues;
        Settings _settings;
        TmdbNetworkClient _client;
        Task<RequestToken> _getToken;
        Task<Task<RequestToken>> _validateToken;

        // Setup:
        // -(1) set up a new unused request token
        // -(2) Validate the request token
        public CreateSessionIdTests(ITestOutputHelper output)
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
        }

        [Fact]
        // happy path
        public async void WhenArgumentsValid_ReturnsNewSessionId()
        {
            // Arrange
            string requestToken = (await (await _validateToken)).Token;
            _output.WriteLine($"Request token successfully {requestToken} created and validated");

            // Act
            var result = await _client.CreateSessionId(requestToken);
            var session = JsonConvert.DeserializeObject<SessionIdResponseModel>(result.Json);            

            // Assert
            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.OK);
            Assert.True(session.Success);
            Assert.False(string.IsNullOrWhiteSpace(session.SessionId));

            _output.WriteLine($"Session id created: {session.SessionId}");
        }

        [Theory]
        // happy path
        [InlineData(3, 1000)]
        public async void WhenArgumentsValidAndCalledWithRetryOption_ReturnsNewSessionId(int retryCount, int delayMilliseconds)
        {
            // Arrange
            string requestToken = (await (await _validateToken)).Token;
            _output.WriteLine($"Request token successfully {requestToken} created and validated");

            // Act
            var result = await _client.CreateSessionId(requestToken, retryCount, delayMilliseconds);
            var session = JsonConvert.DeserializeObject<SessionIdResponseModel>(result.Json);

            // Assert
            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.OK);
            Assert.True(session.Success);
            Assert.False(string.IsNullOrWhiteSpace(session.SessionId));

            _output.WriteLine($"Session id created: {session.SessionId}");
        }

        [Fact]
        public async void WhenTokenInvalid_ReturnsErrorCode()
        {
            // Arrange
            string requestToken = "thisisaninvalidrequesttoken";

            // Act
            var result = await _client.CreateSessionId(requestToken);

            // Assert
            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.Unauthorized);

            _output.WriteLine($"Server responded with status code: {result.HttpStatusCode}");
        }


    }

}

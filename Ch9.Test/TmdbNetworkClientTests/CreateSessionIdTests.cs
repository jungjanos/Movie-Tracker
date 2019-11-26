using Ch9.ApiClient;
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

    // for the critical TmdbNetworkClient.CreateSessionId(...) function accessing the TMDB WebAPI
    // No intention to achieve full coverage for all paths:
    // WebAPI Http 404 error conditions is unclear (when should we receive code 404 ? )
    public class CreateSessionIdTests : IAsyncLifetime
    {
        private string SessionToDispose { get; set; }
        private string ValidatedToken { get; set; }

        private readonly ITestOutputHelper _output;
        Dictionary<string, object> _settingsKeyValues;
        Settings _settings;
        TmdbNetworkClient _client;

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
            _client = new TmdbNetworkClient(_settings, null);
        }

        public async Task InitializeAsync()
        {
            _output.WriteLine("Creating request token: ");
            var createRequestTokenResult = await _client.CreateRequestToken();
            _output.WriteLine($"Server response: {createRequestTokenResult.HttpStatusCode}");
            var token = JsonConvert.DeserializeObject<RequestToken>(createRequestTokenResult.Json);
            _output.WriteLine("Request token validation: ");
            var validateTokenResult = await _client.ValidateRequestTokenWithLogin
                (_settings.AccountName, _settings.Password, token.Token);
            _output.WriteLine($"Server response: {validateTokenResult.HttpStatusCode}");

            string validatedToken = JsonConvert.DeserializeObject<RequestToken>(validateTokenResult.Json).Token;
            ValidatedToken = validatedToken;
        }

        public async Task DisposeAsync()
        {
            _output.WriteLine($"{nameof(DisposeAsync)}: DeleteSession({SessionToDispose}) called...");
            var result = await _client.DeleteSession(SessionToDispose);
            var code = result.HttpStatusCode.ToString();
            _output.WriteLine($"{nameof(DisposeAsync)}: DeleteSession(...) returned with {code}");
        }

        [Fact]
        // happy path
        public async Task WhenArgumentsValid_ReturnsNewSessionId()
        {
            // Act
            var result = await _client.CreateSessionId(ValidatedToken);
            var session = JsonConvert.DeserializeObject<SessionIdResponseModel>(result.Json);
            SessionToDispose = session.SessionId;

            // Assert
            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.OK);
            Assert.True(session.Success);
            Assert.False(string.IsNullOrWhiteSpace(session.SessionId));
            _output.WriteLine($"Session id created: {session.SessionId}");
        }

        [Theory]
        // happy path
        [InlineData(3, 1000)]
        public async Task WhenArgumentsValidAndCalledWithRetryOption_ReturnsNewSessionId(int retryCount, int delayMilliseconds)
        {

            // Act            
            var result = await _client.CreateSessionId(ValidatedToken, retryCount, delayMilliseconds);
            var session = JsonConvert.DeserializeObject<SessionIdResponseModel>(result.Json);
            SessionToDispose = session.SessionId;

            // Assert
            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.OK);
            Assert.True(session.Success);
            Assert.False(string.IsNullOrWhiteSpace(session.SessionId));
            _output.WriteLine($"Session id created: {session.SessionId}");
        }

        [Fact]
        // error path
        public async Task WhenTokenInvalid_ReturnsErrorCode401()
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

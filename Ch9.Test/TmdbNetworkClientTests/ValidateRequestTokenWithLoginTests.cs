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

    // for the critical/fragile TmdbNetworkClient.ValidateRequestTokenWithLogin(...) function accessing the TMDB WebAPI
    // No intention to achieve full coverage for all paths:
    // WebAPI Http 404 error conditions is unclear (when should we receive code 404 ? )
    public class ValidateRequestTokenWithLoginTests
    {
        Dictionary<string, object> _settingsKeyValues;
        Settings _settings;
        TmdbNetworkClient _client;
        Task<RequestToken> _token;
        private readonly ITestOutputHelper _output;

        // We create a fresh request token for each test
        public ValidateRequestTokenWithLoginTests(ITestOutputHelper output)
        {
            this._output = output;

            _settingsKeyValues = new Dictionary<string, object>();
            _settingsKeyValues[nameof(Settings.ApiKey)] = "764d596e888359d26c0dc49deffecbb3";
            _settingsKeyValues[nameof(Settings.AccountName)] = "j4nitest";
            _settingsKeyValues[nameof(Settings.Password)] = "awx123.";
            _settings = new Settings(_settingsKeyValues);
            _client = new TmdbNetworkClient(_settings);

            Task<CreateRequestTokenResult> result = _client.CreateRequestToken();

            _token = result.ContinueWith(t =>
            {
                var json = t.Result;
                RequestToken token = JsonConvert.DeserializeObject<RequestToken>(json.Json);
                return token;
            });
        }

        [Fact]
        // Happy path
        public async void WhenCalledWithValidArguments_ReturnsSuccessAndRequestToken()
        {
            // Arrange
            string requestToken = (await _token).Token;

            // Act
            var result = await _client.ValidateRequestTokenWithLogin(
                _settings.AccountName,
                _settings.Password,
                requestToken);

            _output.WriteLine($"response from server: {result.Json}");
            RequestToken token = JsonConvert.DeserializeObject<RequestToken>(result.Json);

            // Assert
            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.OK);
            Assert.True(token.Success);
            Assert.True(token.Token == requestToken);
        }

        [Fact]
        // Happy path
        public async void WhenCalledWhenAlreadyAuthorized_ReturnsSuccessAndRequestTokenAndDoesNotBreak()
        {
            // Arrange (do a first successfull authorization)

            string requestToken = (await _token).Token;

            var result = await _client.ValidateRequestTokenWithLogin(
                _settings.AccountName,
                _settings.Password,
                requestToken
            );

            RequestToken token = JsonConvert.DeserializeObject<RequestToken>(result.Json);
            if (result.HttpStatusCode != System.Net.HttpStatusCode.OK 
                || !token.Success  || token.Token != requestToken)
            {
                _output.WriteLine($"Problem at test setup, first call returned unexpected result");
                throw new System.Exception("Problem at test setup, first call returned unexpected result");
            }

            // Act (try to authorize again)

            var result2 = await _client.ValidateRequestTokenWithLogin(
                _settings.AccountName,
                _settings.Password,
                requestToken
            );
            RequestToken token2 = JsonConvert.DeserializeObject<RequestToken>(result2.Json);
            _output.WriteLine($"response from server: {result2.Json}");

            // Assert
            Assert.True(result2.HttpStatusCode == System.Net.HttpStatusCode.OK);
            Assert.True(token2.Success);
            Assert.True(token2.Token == requestToken);
        }

        [Theory]
        [InlineData(0, 1000)]
        [InlineData(3, 1000)]
        // error path
        public async void WhenCalledWithInValidApiKeyAndRetryOption_ReturnsCode401AndDoesNotBreak(int retryCount, int delayMilliseconds)
        {
            // Arrange
            _settings.ApiKey = "invalidkeytestcase";
            string requestToken = (await _token).Token;

            // Act
            var result = await _client.ValidateRequestTokenWithLogin(
                _settings.AccountName,
                _settings.Password,
                requestToken,
                retryCount,
                delayMilliseconds
                );

            // Assert
            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.Unauthorized);
        }

        [Fact]
        // error path
        public async void WhenCalledWithInvalidUserName_ReturnsErrorCode_()
        {
            // Arrange
            _settings.AccountName = "InvalidName";
            string requestToken = (await _token).Token;

            // Act
            var result = await _client.ValidateRequestTokenWithLogin(
                _settings.AccountName,
                _settings.Password,
                requestToken
                );

            _output.WriteLine($"Server responded with Http code: {result.HttpStatusCode}");

            // Assert
            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.Unauthorized);
        }
    }
}

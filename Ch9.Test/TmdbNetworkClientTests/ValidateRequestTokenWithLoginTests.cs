using Ch9.ApiClient;
using Ch9.Services;
using Ch9.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Ch9.Test.TmdbNetworkClientTests
{
    // INTEGRATION TESTS

    // for the critical/fragile TmdbNetworkClient.ValidateRequestTokenWithLogin(...) function accessing the TMDB WebAPI
    // No intention to achieve full coverage for all paths:
    // WebAPI Http 404 error conditions is unclear (when should we receive code 404 ? )
    public class ValidateRequestTokenWithLoginTests : IAsyncLifetime
    {
        private string RequestToken { get; set; }

        Dictionary<string, object> _settingsKeyValues;
        Settings _settings;
        TmdbNetworkClient _client;
        private readonly ITestOutputHelper _output;

        // We create a fresh request token for each test
        public ValidateRequestTokenWithLoginTests(ITestOutputHelper output)
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
                _output.WriteLine($"{nameof(InitializeAsync)}: called {nameof(_client.CreateRequestToken)}");
                var createRequestTokenResult = await _client.CreateRequestToken();

                _output.WriteLine($"Server responded: {createRequestTokenResult.HttpStatusCode}, message: {createRequestTokenResult?.Json ?? "null"}");
                var token = JsonConvert.DeserializeObject<RequestToken>(createRequestTokenResult.Json);
                _output.WriteLine($"token created: {token.Token}");
                RequestToken = token.Token;
            }
            catch (Exception ex)
            {
                _output.WriteLine(ex.Message);
                _output.WriteLine(ex.StackTrace);
            }
        }

        public Task DisposeAsync()
        { return Task.CompletedTask; }


        [Fact]
        // Happy path
        public async Task WhenCalledWithValidArguments_ReturnsSuccessAndRequestToken()
        {
            // Act
            var result = await _client.ValidateRequestTokenWithLogin(
                _settings.AccountName,
                _settings.Password,
                RequestToken
                );

            _output.WriteLine($"response from server: {result.Json}");
            RequestToken token = JsonConvert.DeserializeObject<RequestToken>(result.Json);

            // Assert
            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.OK);
            Assert.True(token.Success);
            Assert.True(token.Token == RequestToken);
        }

        [Fact]
        // Happy path
        public async Task WhenCalledWhenAlreadyAuthorized_ReturnsSuccessAndRequestTokenAndDoesNotBreak()
        {
            // Arrange (do first a successfull authorization)
            var result = await _client.ValidateRequestTokenWithLogin(
                _settings.AccountName,
                _settings.Password,
                RequestToken
            );

            RequestToken token = JsonConvert.DeserializeObject<RequestToken>(result.Json);
            if (result.HttpStatusCode != System.Net.HttpStatusCode.OK
                || !token.Success || token.Token != RequestToken)
            {
                _output.WriteLine($"Problem at test setup, first call returned unexpected result");
                throw new System.Exception("Problem at test setup, first call returned unexpected result");
            }

            // Act (try to authorize again)
            var result2 = await _client.ValidateRequestTokenWithLogin(
                _settings.AccountName,
                _settings.Password,
                RequestToken
            );
            RequestToken token2 = JsonConvert.DeserializeObject<RequestToken>(result2.Json);
            _output.WriteLine($"response from server: {result2.Json}");

            // Assert
            Assert.True(result2.HttpStatusCode == System.Net.HttpStatusCode.OK);
            Assert.True(token2.Success);
            Assert.True(token2.Token == RequestToken);
        }

        [Theory]
        [InlineData(0, 1000)]
        [InlineData(3, 1000)]
        // error path
        public async Task WhenCalledWithInValidApiKeyAndRetryOption_ReturnsCode401AndDoesNotBreak(int retryCount, int delayMilliseconds)
        {
            // Arrange
            _settings.ApiKey = "invalidkeytestcase";

            // Act
            var result = await _client.ValidateRequestTokenWithLogin(
                _settings.AccountName,
                _settings.Password,
                RequestToken,
                retryCount,
                delayMilliseconds
                );

            // Assert
            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.Unauthorized);
        }

        [Fact]
        // error path
        public async Task WhenCalledWithInvalidUserName_ReturnsErrorCode_()
        {
            // Arrange
            _settings.AccountName = "InvalidName";

            // Act
            var result = await _client.ValidateRequestTokenWithLogin(
                _settings.AccountName,
                _settings.Password,
                RequestToken
                );

            _output.WriteLine($"Server responded with Http code: {result.HttpStatusCode}");

            // Assert
            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.Unauthorized);
        }
    }
}

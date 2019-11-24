using System.Collections.Generic;
using Ch9.Models;
using Ch9.ApiClient;
using Xunit;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Xunit.Abstractions;
using System;
using Ch9.Services;

namespace Ch9.Test.TmdbNetworkClientTests
{
    // INTEGRATION TESTS

    // for the critical TmdbNetworkClient.GetAccountDetails(...) function accessing the TMDB WebAPI
    // No intention to achieve full coverage for all paths:
    // WebAPI Http 404 error conditions is unclear (when should we receive code 404 ? )
    public class GetAccountDetailsTests : IAsyncLifetime
    {
        private string SessionId { get; set; }

        private readonly ITestOutputHelper _output;
        Dictionary<string, object> _settingsKeyValues;
        Settings _settings;
        TmdbNetworkClient _client;

        public GetAccountDetailsTests(ITestOutputHelper output)
        {
            _output = output;

            _settingsKeyValues = new Dictionary<string, object>();
            _settingsKeyValues[nameof(Settings.ApiKey)] = "764d596e888359d26c0dc49deffecbb3";
            _settingsKeyValues[nameof(Settings.AccountName)] = "j4nitest";
            _settingsKeyValues[nameof(Settings.Password)] = "awx123.";
            _settings = new Settings(_settingsKeyValues);
            _client = new TmdbNetworkClient(_settings, null);
        }

        // Setup steps:
        // -(1) set up a new unused request token
        // -(2) Validate the request token
        // -(3) Create a new session
        public async Task InitializeAsync()
        {
            var tokenResponse = await _client.CreateRequestToken();
            var token = JsonConvert.DeserializeObject<RequestToken>(tokenResponse.Json);
            await _client.ValidateRequestTokenWithLogin
                (_settings.AccountName, _settings.Password, token.Token);

            var createSessionIdResult = await _client.CreateSessionId(token.Token);
            SessionId = JsonConvert.DeserializeObject<SessionIdResponseModel>(createSessionIdResult.Json).SessionId;
        }

        public async Task DisposeAsync()
        {
            _output.WriteLine($"{nameof(DisposeAsync)}: DeleteSession({SessionId}) called...");
            var result = await _client.DeleteSession(SessionId);
            var code = result.HttpStatusCode.ToString();
            _output.WriteLine($"{nameof(DisposeAsync)}: DeleteSession(...) returned with {code}");
        }

        [Fact]
        // happy path
        public async Task WhenValidArguments_ReturnsAccountDetails()
        {
            // Act
            var result = await _client.GetAccountDetails(SessionId);
            PrintResultStatusToLog(result);

            AccountDetailsModel account = JsonConvert.DeserializeObject<AccountDetailsModel>(result?.Json);
            PrintAccountDetailsToLog(account);

            // Assert
            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.OK);
            Assert.True(account.AccountName == _settings.AccountName);
        }

        [Theory]
        [InlineData(3, 1000)]
        // happy path
        public async Task WhenValidArgumentsAndCalledWithRetryOption_ReturnsAccountDetails(int retryCount, int delayMilliseconds)
        {
            // Act
            var result = await _client.GetAccountDetails(SessionId, retryCount, delayMilliseconds);
            PrintResultStatusToLog(result);

            AccountDetailsModel account = JsonConvert.DeserializeObject<AccountDetailsModel>(result?.Json);
            PrintAccountDetailsToLog(account);

            // Assert
            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.OK);
            Assert.True(account.AccountName == _settings.AccountName);
        }

        [Fact]
        // error path
        public async Task WhenValidInvalidSessionId_ReturnsErrorCode401()
        {
            // Arrange
            var invalidSessionId = "someinvalidsessionid";

            // Act
            var result = await _client.GetAccountDetails(invalidSessionId);
            PrintResultStatusToLog(result);

            // Assert
            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.Unauthorized);
        }

        private void PrintResultStatusToLog(GetAccountDetailsResult result)
        {
            _output.WriteLine($"GetAccountDetails(sessionId: {SessionId}) returned:");
            _output.WriteLine($"{result?.Json}");
            _output.WriteLine(Environment.NewLine);
        }
        private void PrintAccountDetailsToLog(AccountDetailsModel account)
        {
            _output.WriteLine($"Account details:");
            _output.WriteLine($"id: \t\t{account?.Id}");
            _output.WriteLine($"account: \t\t{account?.AccountName}");
            _output.WriteLine($"Name: \t\t{account?.Name}");
            _output.WriteLine($"include adult: \t{account?.IncludeAdult}");
            _output.WriteLine($"ISO639: \t\t{account?.Iso639}");
            _output.WriteLine($"ISO3166: \t{account?.Iso3166}");
        }
    }
}

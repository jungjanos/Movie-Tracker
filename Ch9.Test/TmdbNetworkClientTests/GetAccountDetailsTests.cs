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

    // for the critical TmdbNetworkClient.GetAccountDetails(...) function accessing the TMDB WebAPI
    // No intention to achieve full coverage for all paths:
    // WebAPI Http 404 error conditions is unclear (when should we receive code 404 ? )
    public class GetAccountDetailsTests : IDisposable
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
        public GetAccountDetailsTests(ITestOutputHelper output)
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

        // Teardown
        public async void Dispose()
        {
            var sessionId = (await await _getSession).SessionId;
            _output.WriteLine($"Teardown: DeleteSession({sessionId}) called...");
            var result = await _client.DeleteSession(sessionId);
            var code = result == null ? "ERROR" : result.HttpStatusCode.ToString();
            _output.WriteLine($"Teardown: DeleteSession(...) returned with {code}");
        }

        [Fact]
        // happy path
        public async void WhenValidArguments_ReturnsAccountDetails()
        {
            // Arrange
            var sessionId = (await await _getSession).SessionId;

            try
            {
                // Act
                var result = await _client.GetAccountDetails(sessionId);
                {
                    _output.WriteLine($"GetAccountDetails(sessionId: {sessionId}) returned:");
                    _output.WriteLine($"{result.Json}");
                    _output.WriteLine(Environment.NewLine);
                }

                AccountDetailsModel account = JsonConvert.DeserializeObject<AccountDetailsModel>(result.Json);
                {
                    _output.WriteLine($"Account details:");
                    _output.WriteLine($"id: \t\t{account?.Id}");
                    _output.WriteLine($"account: \t\t{account?.AccountName}");
                    _output.WriteLine($"Name: \t\t{account?.Name}");
                    _output.WriteLine($"include adult: \t{account?.IncludeAdult}");
                    _output.WriteLine($"ISO639: \t\t{account?.Iso639}");
                    _output.WriteLine($"ISO3166: \t{account?.Iso3166}");
                }
                // Assert
                Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.OK);
                Assert.True(account.AccountName == _settings.AccountName);
            }
            catch (Exception ex)
            {
                _output.WriteLine(ex.Message);
                _output.WriteLine(ex.StackTrace);
            }
        }

        [Theory]
        [InlineData(3, 1000)]
        // happy path
        public async void WhenValidArgumentsAndCalledWithRetryOption_ReturnsAccountDetails(int retryCount, int delayMilliseconds)
        {
            // Arrange
            var sessionId = (await await _getSession).SessionId;

            try
            {
                // Act
                var result = await _client.GetAccountDetails(sessionId, retryCount, delayMilliseconds);
                {
                    _output.WriteLine($"GetAccountDetails(sessionId: {sessionId}) returned:");
                    _output.WriteLine($"{result.Json}");
                    _output.WriteLine(Environment.NewLine);
                }

                AccountDetailsModel account = JsonConvert.DeserializeObject<AccountDetailsModel>(result.Json);
                {
                    _output.WriteLine($"Account details:");
                    _output.WriteLine($"id: \t\t{account?.Id}");
                    _output.WriteLine($"account: \t\t{account?.AccountName}");
                    _output.WriteLine($"Name: \t\t{account?.Name}");
                    _output.WriteLine($"include adult: \t{account?.IncludeAdult}");
                    _output.WriteLine($"ISO639: \t\t{account?.Iso639}");
                    _output.WriteLine($"ISO3166: \t{account?.Iso3166}");
                }

                // Assert
                Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.OK);
                Assert.True(account.AccountName == _settings.AccountName);
            }
            catch (Exception ex)
            {
                _output.WriteLine(ex.Message);
                _output.WriteLine(ex.StackTrace);
            }
        }


        [Fact]
        // error path
        public async void WhenValidInvalidSessionId_ReturnsErrorCode401()
        {
            // Arrange
            var sessionId = "someinvalidsessionid";

            try
            {
                // Act
                var result = await _client.GetAccountDetails(sessionId);
                {
                    _output.WriteLine($"GetAccountDetails(sessionId: {sessionId}) returned:");
                    _output.WriteLine($"Error code: {result.HttpStatusCode}");
                    _output.WriteLine(Environment.NewLine);
                }

                // Assert
                Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.Unauthorized);                
            }
            catch (Exception ex)
            {
                _output.WriteLine(ex.Message);
                _output.WriteLine(ex.StackTrace);
            }
        }
    }
}

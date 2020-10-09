using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Ch9.Services.LocalSettings;
using Ch9.Data.ApiClient;
using Ch9.Data.Contracts;

namespace Ch9.Test.TmdbNetworkClientTests
{
    // INTEGRATION TESTS
    // for the critical TmdbNetworkClient.RateMovie(...) function accessing the TMDB WebAPI
    public class RateMovieTests : IAsyncLifetime
    {
        private readonly ITestOutputHelper _output;
        readonly Dictionary<string, object> _settingsKeyValues;
        readonly Settings _settings;
        readonly TmdbNetworkClient _client;

        readonly int _movie = 60800; // Macskafogó
        readonly int _invalidMovie = 99999999;

        public RateMovieTests(ITestOutputHelper output)
        {
            _output = output;
            _settingsKeyValues = new Dictionary<string, object>();
            _settingsKeyValues[nameof(Settings.ApiKey)] = "764d596e888359d26c0dc49deffecbb3";
            _settingsKeyValues[nameof(Settings.SessionId)] = "563636d0e4a0b41b775ba7703cc5c985f36cffaf"; // !!!! correct it !!!!!
            _settings = new Settings(_settingsKeyValues, null);
            _client = new TmdbNetworkClient(null, _settings.ApiKey);
        }

        public async Task InitializeAsync()
        {
            await _client.DeleteMovieRating(_settings.SessionId, _movie);
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        // happy path
        public async Task WhenCalledOnUnratedMovie_RatesMovie()
        {
            decimal rating = 10m;
            RateMovieResult response = await _client.RateMovie(_settings.SessionId, rating: rating, mediaId: _movie, guestSessionId: null, retryCount: 0, delayMilliseconds: 1000);

            _output.WriteLine($"TMDB server's response code {response.HttpStatusCode}");
            _output.WriteLine($"TMDB server's response message {response.Json}");
            Assert.True(response.HttpStatusCode == System.Net.HttpStatusCode.Created);
        }
        [Fact]
        // happy path
        public async Task WhenCalledOnAlreadyRatedMovieWithSameRating_ReturnsSuccess()
        {
            decimal rating = 10m;
            await _client.RateMovie(_settings.SessionId, rating: rating, mediaId: _movie, guestSessionId: null, retryCount: 0, delayMilliseconds: 1000);

            RateMovieResult response = await _client.RateMovie(_settings.SessionId, rating: rating, mediaId: _movie, guestSessionId: null, retryCount: 0, delayMilliseconds: 1000);

            _output.WriteLine($"TMDB server's response code {response.HttpStatusCode}");
            _output.WriteLine($"TMDB server's response message {response.Json}");
            Assert.True(response.HttpStatusCode == System.Net.HttpStatusCode.Created);
        }

        [Fact]
        // happy path
        public async Task WhenCalledOnAlreadyRatedMovieWithDifferentRating_ReturnsSuccessUpdates()
        {
            decimal rating1 = 7.5m;
            decimal rating2 = 9.5m;
            await _client.RateMovie(_settings.SessionId, rating: rating1, mediaId: _movie, guestSessionId: null, retryCount: 0, delayMilliseconds: 1000);

            RateMovieResult response = await _client.RateMovie(_settings.SessionId, rating: rating2, mediaId: _movie, guestSessionId: null, retryCount: 0, delayMilliseconds: 1000);

            _output.WriteLine($"TMDB server's response code {response.HttpStatusCode}");
            _output.WriteLine($"TMDB server's response message {response.Json}");
            Assert.True(response.HttpStatusCode == System.Net.HttpStatusCode.Created);
        }

        [Fact]
        // failure path
        public async Task WhenCalledWithInvalidSessionId_ReturnsErrorCode401()
        {
            decimal rating = 10m;
            _settings.SessionId = "invalidSessionId";

            RateMovieResult response = await _client.RateMovie(_settings.SessionId, rating: rating, mediaId: _movie, guestSessionId: null, retryCount: 0, delayMilliseconds: 1000);

            _output.WriteLine($"TMDB server's response code {response.HttpStatusCode}");
            Assert.True(response.HttpStatusCode == System.Net.HttpStatusCode.Unauthorized);
        }

        [Fact]
        // failure path
        public async Task WhenCalledWithInvalidMovieId_ReturnsErrorCode404()
        {
            decimal rating = 5.5m;

            RateMovieResult response = await _client.RateMovie(_settings.SessionId, rating: rating, mediaId: _invalidMovie, guestSessionId: null, retryCount: 0, delayMilliseconds: 1000);

            _output.WriteLine($"TMDB server's response code {response.HttpStatusCode}");
            Assert.True(response.HttpStatusCode == System.Net.HttpStatusCode.NotFound);
        }
    }
}

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
    // for the critical TmdbNetworkClient.DeleteMovieRating(...) function accessing the TMDB WebAPI
    public class DeleteMovieRatingTests : IAsyncLifetime
    {
        private readonly ITestOutputHelper _output;
        readonly Dictionary<string, object> _settingsKeyValues;
        readonly Settings _settings;
        readonly TmdbNetworkClient _client;

        readonly int _movie = 60800; // Macskafogó
        readonly int _invalidMovie = 99999999;

        public DeleteMovieRatingTests(ITestOutputHelper output)
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
            await _client.RateMovie(_settings.SessionId, 10m, _movie);
            await Task.Delay(500);
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        // happy path
        public async Task WhenCalledOnRatedMovie_DeletesRating()
        {
            DeleteMovieRatingResult response = await _client.DeleteMovieRating(_settings.SessionId, mediaId: _movie, guestSessionId: null, retryCount: 0);

            _output.WriteLine($"TMDB server's response code {response.HttpStatusCode}");
            _output.WriteLine($"TMDB server's response {response.Json}");
            Assert.True(response.HttpStatusCode == System.Net.HttpStatusCode.OK);
        }

        [Fact]
        // happy path
        public async Task WhenCalledOnUnratedMovie_Returns200()
        {
            await _client.DeleteMovieRating(_settings.SessionId, mediaId: _movie, guestSessionId: null, retryCount: 0);
            DeleteMovieRatingResult response = await _client.DeleteMovieRating(_settings.SessionId, mediaId: _movie, guestSessionId: null, retryCount: 0);

            _output.WriteLine($"TMDB server's response code {response.HttpStatusCode}");
            _output.WriteLine($"TMDB server's response {response.Json}");
            Assert.True(response.HttpStatusCode == System.Net.HttpStatusCode.OK);
        }

        [Fact]
        // failure path
        public async Task WhenCalledWithInvalidMovieId_Returns404()
        {
            DeleteMovieRatingResult response = await _client.DeleteMovieRating(_settings.SessionId, mediaId: _invalidMovie, guestSessionId: null, retryCount: 0);

            _output.WriteLine($"TMDB server's response code {response.HttpStatusCode}");
            Assert.True(response.HttpStatusCode == System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        // failure path
        public async Task WhenCalledWithInvalidSessionId_Returns401()
        {
            _settings.SessionId = "invalidSessionId";
            DeleteMovieRatingResult response = await _client.DeleteMovieRating(_settings.SessionId, mediaId: _movie, guestSessionId: null, retryCount: 0);

            _output.WriteLine($"TMDB server's response code {response.HttpStatusCode}");
            Assert.True(response.HttpStatusCode == System.Net.HttpStatusCode.Unauthorized);
        }
    }
}

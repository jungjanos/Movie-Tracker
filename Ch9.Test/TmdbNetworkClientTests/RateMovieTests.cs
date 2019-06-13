using Ch9.ApiClient;
using Ch9.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

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
            _settings = new Settings(_settingsKeyValues);
            _client = new TmdbNetworkClient(_settings);
        }

        public async Task InitializeAsync()
        {
            await _client.DeleteMovieRating(_movie);
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        // happy path
        public async Task WhenCalledOnUnratedMovie_RatesMovie()
        {
            Rating rating = Rating.Ten;
            RateMovieResult response = await _client.RateMovie(rating: rating, mediaId: _movie, guestSessionId: null, retryCount: 0, delayMilliseconds: 1000);

            _output.WriteLine($"TMDB server's response code {response.HttpStatusCode}");
            _output.WriteLine($"TMDB server's response message {response.Json}");
            Assert.True(response.HttpStatusCode == System.Net.HttpStatusCode.Created);          
        }
        [Fact]
        // happy path
        public async Task WhenCalledOnAlreadyRatedMovieWithSameRating_ReturnsSuccess()
        {
            Rating rating = Rating.Ten;
            await _client.RateMovie(rating: rating, mediaId: _movie, guestSessionId: null, retryCount: 0, delayMilliseconds: 1000);

            RateMovieResult response = await _client.RateMovie(rating: rating, mediaId: _movie, guestSessionId: null, retryCount: 0, delayMilliseconds: 1000);

            _output.WriteLine($"TMDB server's response code {response.HttpStatusCode}");
            _output.WriteLine($"TMDB server's response message {response.Json}");
            Assert.True(response.HttpStatusCode == System.Net.HttpStatusCode.Created);
        }

        [Fact]
        // happy path
        public async Task WhenCalledOnAlreadyRatedMovieWithDifferentRating_ReturnsSuccessUpdates()
        {
            Rating rating1 = Rating.SevenAndHalf;
            Rating rating2 = Rating.NineAndHalf;
            await _client.RateMovie(rating: rating1, mediaId: _movie, guestSessionId: null, retryCount: 0, delayMilliseconds: 1000);

            RateMovieResult response = await _client.RateMovie(rating: rating2, mediaId: _movie, guestSessionId: null, retryCount: 0, delayMilliseconds: 1000);

            _output.WriteLine($"TMDB server's response code {response.HttpStatusCode}");
            _output.WriteLine($"TMDB server's response message {response.Json}");
            Assert.True(response.HttpStatusCode == System.Net.HttpStatusCode.Created);
        }      

        [Fact]
        // failure path
        public async Task WhenCalledWithInvalidSessionId_ReturnsErrorCode401()
        {
            Rating rating = Rating.Ten;
            _settings.SessionId = "invalidSessionId";

            RateMovieResult response = await _client.RateMovie(rating: rating, mediaId: _movie, guestSessionId: null, retryCount: 0, delayMilliseconds: 1000);

            _output.WriteLine($"TMDB server's response code {response.HttpStatusCode}");
            Assert.True(response.HttpStatusCode == System.Net.HttpStatusCode.Unauthorized);
        }

        [Fact]
        // failure path
        public async Task WhenCalledWithInvalidMovieId_ReturnsErrorCode404()
        {
            Rating rating = Rating.FiveAndHalf;

            RateMovieResult response = await _client.RateMovie(rating: rating, mediaId: _invalidMovie, guestSessionId: null, retryCount: 0, delayMilliseconds: 1000);

            _output.WriteLine($"TMDB server's response code {response.HttpStatusCode}");
            Assert.True(response.HttpStatusCode == System.Net.HttpStatusCode.NotFound);
        }
    }
}

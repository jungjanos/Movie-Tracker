using Ch9.ApiClient;
using Ch9.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Ch9.Test.TmdbNetworkClientTests
{
    // INTEGRATION TESTS
    // for the critical TmdbNetworkClient.GetMovieVideos(...) function accessing the TMDB WebAPI
    public class GetMovieVideosTests
    {
        private readonly ITestOutputHelper _output;
        readonly Dictionary<string, object> _settingsKeyValues;
        readonly Settings _settings;
        readonly TmdbNetworkClient _client;
        readonly int _movie = 399579;
        readonly int _movieWithoutVideos = 131223;
        readonly int _movieWithout_hu_Videos = 114013; 

        readonly int _invalidMovieId = 99999999;
        public GetMovieVideosTests(ITestOutputHelper output)
        {
            _output = output;

            _settingsKeyValues = new Dictionary<string, object>();
            _settingsKeyValues[nameof(Settings.ApiKey)] = "764d596e888359d26c0dc49deffecbb3";

            _settings = new Settings(_settingsKeyValues);
            _client = new TmdbNetworkClient(_settings, null);
        }

        [Fact]
        // happy path
        public async Task WhenCalledOnMovieWithVideos_ReturnsOK()
        {
            var result = await _client.GetMovieVideos(_movie, language: null);

            _output.WriteLine($"Server returned {result.HttpStatusCode}");
            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.OK);
        }

        [Fact]
        // happy path
        public async Task WhenCalledOnMovieWithOutVideos_ReturnsOK()
        {
            var result = await _client.GetMovieVideos(_movieWithoutVideos, language: null);

            _output.WriteLine($"Server returned {result.HttpStatusCode}");
            _output.WriteLine($"Json: {result.Json}");

            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.OK);
        }

        [Fact]
        // failure path
        public async Task WhenCalledOnInvalidMovie_Returns404()
        {
            var result = await _client.GetMovieVideos(_invalidMovieId, language: null);

            _output.WriteLine($"Server returned {result.HttpStatusCode}");            

            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        // happy path
        public async Task WhenCalledOnMovieWithVideos_ReturnsCollection()
        {
            var result = await _client.GetMovieVideos(_movie, language: null);                        

            JsonConvert.PopulateObject(result.Json, result);
            PrintVideoDetails(result);

            Assert.True(result.VideoModels.Count > 0);
        }

        [Fact]
        // happy path
        public async Task WhenCalledWithLanguageOption_ReturnsResultsInSpecificLanguage()
        {
            var language = "hu";
            var result = await _client.GetMovieVideos(_movie, language: language);

            JsonConvert.PopulateObject(result.Json, result);
            PrintVideoDetails(result);

            bool resultsInCorrectLanguage =  result.VideoModels.All(videoDetail => videoDetail.Iso == language);

            Assert.True(resultsInCorrectLanguage);
        }

        [Fact]
        // happy path
        public async Task WhenNoVideosInRequestedLanguage_DoesNotFallBack_ReturnsEmpty()
        {
            var language = "hu";
            var result = await _client.GetMovieVideos(_movieWithout_hu_Videos, language: language);

            JsonConvert.PopulateObject(result.Json, result);
            PrintVideoDetails(result);            

            Assert.True(result.VideoModels.Count == 0);
        }

        private void PrintVideoDetails(GetMovieVideosResult movieVideos)
        {
            _output.WriteLine($"{movieVideos.VideoModels.Count} videos contained");

            foreach (var videoDetail in movieVideos.VideoModels)
            {
                _output.WriteLine("=======================");
                _output.WriteLine($"Movie id: {videoDetail.Id}");
                _output.WriteLine($"iso: {videoDetail.Iso}");
                _output.WriteLine($"key: {videoDetail.Key}");
                _output.WriteLine($"name : {videoDetail.Name}");
                _output.WriteLine($"site: {videoDetail.Site}");
                _output.WriteLine($"video size: {videoDetail.Size}");
                _output.WriteLine($"video type: {videoDetail.Type}");
            }
        }
    }
}

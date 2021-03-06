﻿using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;
using Ch9.Models;
using Ch9.Services.LocalSettings;
using Ch9.Data.ApiClient;

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

            _settings = new Settings(_settingsKeyValues, null);
            _client = new TmdbNetworkClient(null, _settings.ApiKey);
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
            var tmdbVideosModel = JsonConvert.DeserializeObject<GetMovieVideosModel>(result.Json);

            PrintVideoDetails(tmdbVideosModel.VideoModels);

            Assert.True(tmdbVideosModel.VideoModels.Count > 0);
        }

        [Fact]
        // happy path
        public async Task WhenCalledWithLanguageOption_ReturnsResultsInSpecificLanguage()
        {
            var language = "hu";
            var result = await _client.GetMovieVideos(_movie, language: language);
            var tmdbVideosModel = JsonConvert.DeserializeObject<GetMovieVideosModel>(result.Json);

            PrintVideoDetails(tmdbVideosModel.VideoModels);

            bool resultsInCorrectLanguage = tmdbVideosModel.VideoModels.All(videoDetail => videoDetail.Iso == language);

            Assert.True(resultsInCorrectLanguage);
        }

        [Fact]
        // happy path
        public async Task WhenNoVideosInRequestedLanguage_DoesNotFallBack_ReturnsEmpty()
        {
            var language = "hu";
            var result = await _client.GetMovieVideos(_movieWithout_hu_Videos, language: language);
            var tmdbVideosModel = JsonConvert.DeserializeObject<GetMovieVideosModel>(result.Json);

            PrintVideoDetails(tmdbVideosModel.VideoModels);

            Assert.True(tmdbVideosModel.VideoModels.Count == 0);
        }

        private void PrintVideoDetails(List<TmdbVideoModel> movieVideos)
        {
            _output.WriteLine($"{movieVideos.Count} videos contained");

            foreach (var videoDetail in movieVideos)
            {
                _output.WriteLine("=======================");
                _output.WriteLine($"Movie id: {videoDetail.Id}");
                _output.WriteLine($"iso: {videoDetail.Iso}");
                _output.WriteLine($"key: {videoDetail.Key}");
                _output.WriteLine($"name : {videoDetail.Title}");
                _output.WriteLine($"site: {videoDetail.Site}");
                _output.WriteLine($"video size: {videoDetail.Size}");
                _output.WriteLine($"video type: {videoDetail.TypeStr}");
            }
        }
    }
}

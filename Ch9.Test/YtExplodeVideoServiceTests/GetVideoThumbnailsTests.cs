using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Ch9.Services.VideoService;
using Ch9.Models;
using Ch9.Services.LocalSettings;
using Ch9.Data.ApiClient;
using Ch9.Services.ApiCommunicationService;

namespace Ch9.Test.YtExplodeVideoServiceTests
{
    // INTEGRATION TESTS

    // for the critical YtExplodeVideoService.GetVideoThumbnails(...) function accessing the YoutubeExplode
    // API (ripping thumbnails and video data from Youtube)

    public class GetVideoThumbnailsTests
    {
        private readonly ITestOutputHelper _output;
        private YtExplodeVideoService _ytExplodeVideoService;
        private readonly string _englishLanguage = "en";

        private int _movieWithYtTrailers = 301528; // toy story 4
        private int _movieWithoutYtTrailersInSearchLanguage = 335457;
        private int _invalidMovieId = 9999999;

        public GetVideoThumbnailsTests(ITestOutputHelper output)
        {
            _output = output;
            var settingsKeyValues = new Dictionary<string, object>();
            settingsKeyValues[nameof(Settings.SearchLanguage)] = _englishLanguage;
            settingsKeyValues[nameof(Settings.PlaybackQuality)] = VideoPlaybackQuality.High.ToString();

            var settings = new Settings(settingsKeyValues, null);
            var tmdbCachedSearchClient = new TmdbCachedSearchClient(new TmdbNetworkClient(new System.Net.Http.HttpClient(), settings.ApiKey));
            _ytExplodeVideoService = new YtExplodeVideoService(null, settings, new TmdbApiService(tmdbCachedSearchClient, settings), null);
        }

        [Fact]
        // happy path
        public async Task WhenCalledOnMovieWithYtTrailers_ReturnsThumbnailsCorrectlyPopulated()
        {
            List<ImageModel> thumbnails = await _ytExplodeVideoService.GetVideoThumbnails(_movieWithYtTrailers, retryCount: 0, delayMilliseconds: 1000, fromCache: false);

            _output.WriteLine($"Fetched {thumbnails.Count} thumbnails");
            foreach (var thumbnail in thumbnails)
                PrintImageDetails(thumbnail);

            Assert.True(thumbnails.Count > 0);
            Assert.True(thumbnails.All(thumb => !string.IsNullOrEmpty(thumb.FilePath)));
            Assert.True(thumbnails.All(thumb => thumb.HasAttachedVideo));
            Assert.True(thumbnails.All(thumb => !string.IsNullOrEmpty(thumb.FilePath)));
            Assert.True(thumbnails.All(thumb => !string.IsNullOrEmpty(thumb.AttachedVideo.Key)));
            Assert.True(thumbnails.All(thumb => thumb.Iso == _englishLanguage));
        }

        [Fact]
        //happy path
        public async Task WhenCalledOnMovieWithoutYtTrailers_ReturnsEmptyCollection()
        {
            List<ImageModel> thumbnails = await _ytExplodeVideoService.GetVideoThumbnails(_movieWithoutYtTrailersInSearchLanguage, retryCount: 0, delayMilliseconds: 1000, fromCache: false);

            _output.WriteLine($"Fetched {thumbnails.Count} thumbnails");
            foreach (var thumbnail in thumbnails)
                PrintImageDetails(thumbnail);

            Assert.True(thumbnails.Count == 0);
        }

        [Fact]
        //happy path
        public async Task WhenCalledOnMovieWithInvalidId_ThrowsException()
        {
            object exception = null;

            try
            {
                await _ytExplodeVideoService.GetVideoThumbnails(_invalidMovieId, retryCount: 0, delayMilliseconds: 1000, fromCache: false);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            Assert.IsType<Exception>(exception);
            Assert.Contains("TMDB", (exception as Exception).Message);
        }

        private void PrintImageDetails(ImageModel image)
        {
            _output.WriteLine("==================");
            _output.WriteLine($"File path: {image.FilePath}");
            _output.WriteLine($"Has attached video: {image.HasAttachedVideo}");
            _output.WriteLine($"Attached video's Youtube Id : {image.AttachedVideo.Key}");
            _output.WriteLine($"Site (should be 'YouTube'): {image.AttachedVideo.Site}");
            _output.WriteLine($"Language: {image.AttachedVideo.Iso}");
            _output.WriteLine($"Video title: {image.AttachedVideo.Title}");
            _output.WriteLine($"Video type (should be 'trailer'): {image.AttachedVideo.TypeStr}");
        }
    }
}

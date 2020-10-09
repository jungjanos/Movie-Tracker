using Ch9.Services.VideoService;
using Ch9.Models;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Ch9.Services.LocalSettings;
using Ch9.Services;
using Ch9.Data.ApiClient;
using Ch9.Services.ApiCommunicationService;

namespace Ch9.Test.YtExplodeVideoServiceTests
{
    public class PlayVideoTests
    {
        private readonly ITestOutputHelper _output;
        private YtExplodeVideoService _ytExplodeVideoService;
        private readonly string _englishLanguage = "en";

        private string _youtubetMovieTrailer = "w7pYhpJaJW8"; // Alita        
        private string _invalidYoutubeKey = "-bBzMeBFqFV";
        private Mock<IPageService> _mockPageService = new Mock<IPageService>();
        private string streamUrlResult = null;

        public PlayVideoTests(ITestOutputHelper output)
        {
            _output = output;
            var settingsKeyValues = new Dictionary<string, object>();
            settingsKeyValues[nameof(Settings.SearchLanguage)] = _englishLanguage;
            settingsKeyValues[nameof(Settings.PlaybackQuality)] = VideoPlaybackQuality.High.ToString();

            var settings = new Settings(settingsKeyValues, null);
            var tmdbCachedSearchClient = new TmdbCachedSearchClient(new TmdbNetworkClient(null, settings.ApiKey));
            //_ytExplodeVideoService = new YtExplodeVideoService(null, settings, tmdbCachedSearchClient);
            _ytExplodeVideoService = new YtExplodeVideoService(null, settings, new TmdbApiService(tmdbCachedSearchClient, settings), null);


            _mockPageService.Setup(ps => ps.PushVideoPageAsync(It.IsAny<string>())).Callback<string>(streamUrl => streamUrlResult = streamUrl);
        }

        // happy path
        [Fact]
        public async Task WhenCalledOnVideoWithValidStreams_CallsPageService()
        {
            // Arrange
            TmdbVideoModel videoModel = new TmdbVideoModel { Key = _youtubetMovieTrailer };
            await _ytExplodeVideoService.PopulateWithStreams(videoModel);

            // Act
            await _ytExplodeVideoService.PlayVideo(videoModel, _mockPageService.Object);

            // Assert
            _mockPageService.Verify(m => m.PushVideoPageAsync(It.Is<string>(str => !string.IsNullOrEmpty(str))), Times.Once);
            _output.WriteLine($"Selected stream Url: {streamUrlResult}");
        }

        //failure
        [Fact]
        public async Task WhenCalledOnVideoWithInvalidVideo_DoesntCallPageService()
        {
            // Arrange
            TmdbVideoModel videoModel = new TmdbVideoModel { Key = _invalidYoutubeKey };
            await _ytExplodeVideoService.PopulateWithStreams(videoModel);

            //Act
            await _ytExplodeVideoService.PlayVideo(videoModel, _mockPageService.Object);

            //Assert
            _mockPageService.Verify(m => m.PushVideoPageAsync(It.IsAny<string>()), Times.Never);
            _output.WriteLine($"Selected stream Url: {streamUrlResult}");
        }
    }
}

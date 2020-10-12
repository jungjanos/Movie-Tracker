using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Moq;
using Ch9.Services.VideoService;
using Ch9.Models;
using Ch9.Services.LocalSettings;
using Ch9.Data.ApiClient;
using Ch9.Services.ApiCommunicationService;
using Ch9.Services.Contracts;

namespace Ch9.Test.YtExplodeVideoServiceTests
{
    public class PlayVideoTests
    {
        private readonly ITestOutputHelper _output;
        private YtExplodeVideoService _ytExplodeVideoService;
        private readonly string _englishLanguage = "en";

        private string _youtubetMovieTrailer = "w7pYhpJaJW8"; // Alita        
        private string _invalidYoutubeKey = "-bBzMeBFqFV";
        private Mock<IPlayVideo> _mockVideoPlayer = new Mock<IPlayVideo>();
        private string streamUrlResult = null;

        public PlayVideoTests(ITestOutputHelper output)
        {
            _output = output;
            var settingsKeyValues = new Dictionary<string, object>();
            settingsKeyValues[nameof(Settings.SearchLanguage)] = _englishLanguage;
            settingsKeyValues[nameof(Settings.PlaybackQuality)] = VideoPlaybackQuality.High.ToString();

            var settings = new Settings(settingsKeyValues, null);
            var tmdbCachedSearchClient = new TmdbCachedSearchClient(new TmdbNetworkClient(null, settings.ApiKey));
            _mockVideoPlayer.Setup(player => player.OpenVideoStreamDirectly(It.IsAny<string>())).Callback<string>(streamUrl => streamUrlResult = streamUrl);

            _ytExplodeVideoService = new YtExplodeVideoService(null, settings, new TmdbApiService(tmdbCachedSearchClient, settings), _mockVideoPlayer.Object);
        }

        // happy path
        [Fact]
        public async Task WhenCalledOnVideoWithValidStreams_CallsPageService()
        {
            // Arrange
            TmdbVideoModel videoModel = new TmdbVideoModel { Key = _youtubetMovieTrailer };
            await _ytExplodeVideoService.PopulateWithStreams(videoModel);

            // Act
            await _ytExplodeVideoService.PlayVideo(videoModel);

            // Assert
            _mockVideoPlayer.Verify(m => m.OpenVideoStreamDirectly(It.Is<string>(str => !string.IsNullOrEmpty(str))), Times.Once);
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
            await _ytExplodeVideoService.PlayVideo(videoModel);

            //Assert
            _mockVideoPlayer.Verify(m => m.OpenVideoStreamDirectly(It.IsAny<string>()), Times.Never);
            _output.WriteLine($"Selected stream Url: {streamUrlResult}");
        }
    }
}

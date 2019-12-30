using Ch9.ApiClient;
using Ch9.Services;
using Ch9.Services.VideoService;
using Ch9.Ui.Contracts;
using Ch9.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Ch9.Test.YtExplodeVideoServiceTests
{
    public class PopulateWithStreamsTests
    {
        private readonly ITestOutputHelper _output;
        private YtExplodeVideoService _ytExplodeVideoService;
        private readonly string _englishLanguage = "en";

        private string _youtubetMovieTrailer = "w7pYhpJaJW8"; // Alita        
        private string _invalidYoutubeKey = "-bBzMeBFqFV";

        public PopulateWithStreamsTests(ITestOutputHelper output)
        {
            _output = output;
            var settingsKeyValues = new Dictionary<string, object>();
            settingsKeyValues[nameof(Settings.SearchLanguage)] = _englishLanguage;
            settingsKeyValues[nameof(Settings.PlaybackQuality)] = VideoPlaybackQuality.High.ToString();

            var settings = new Settings(settingsKeyValues);
            var tmdbCachedSearchClient = new TmdbCachedSearchClient(new TmdbNetworkClient(settings, new System.Net.Http.HttpClient()));
            _ytExplodeVideoService = new YtExplodeVideoService(null, settings, tmdbCachedSearchClient);
        }

        //happy path
        [Fact]
        public async Task WhenCalledOnValidYoutubeId_PopulatesObject()
        {
            TmdbVideoModel videoModel = new TmdbVideoModel { Key = _youtubetMovieTrailer };
            await _ytExplodeVideoService.PopulateWithStreams(videoModel);

            PrintStreamSetDetails(videoModel.Streams);

            Assert.True(videoModel.Streams.VideoStreams.Count() > 0);
        }

        // failure path
        [Fact]
        public async Task WhenCalledOnInvalidYoutubeId_DoesNotThrowPopulatesNull()
        {
            TmdbVideoModel videoModel = new TmdbVideoModel { Key = _invalidYoutubeKey };
            await _ytExplodeVideoService.PopulateWithStreams(videoModel);

            Assert.Null(videoModel.Streams);
        }


        private void PrintStreamSetDetails(VideoStreamInfoSet streamSet)
        {
            _output.WriteLine($"Number of streams: {streamSet.VideoStreams.Count()}");
            _output.WriteLine($"List of streams: ");
            foreach (var stream in streamSet.VideoStreams)
                PrintStreamDetails(stream);
        }

        private void PrintStreamDetails(VideoStreamInfo stream)
        {
            _output.WriteLine("------------");
            _output.WriteLine($"Url: {stream.StreamUrl}");
            _output.WriteLine($"Quality label: {stream.QualityLabel}");
            _output.WriteLine($"Quality: {stream.Quality}");
            _output.WriteLine($"Height: {stream.Height}");
            _output.WriteLine($"Width: {stream.Width}");
        }
    }
}

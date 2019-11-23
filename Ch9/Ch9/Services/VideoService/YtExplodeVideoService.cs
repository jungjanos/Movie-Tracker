#if !GOOGLEPLAY
using Ch9.ApiClient;
using Ch9.Models;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Models.MediaStreams;

namespace Ch9.Services.VideoService
{    
    /// <summary>
    /// Video service implementation based on YoutubeExplode. 
    /// Extracts stream information and lets play videos ads free without a Youtube Api key. 
    /// YoutubeExplode is responsible to reverse engineering YT videos from a youtube video-id.
    /// Nem Google play kompatibilis :) 
    /// </summary>        
    public class YtExplodeVideoService : VideoServiceBase, IVideoService
    {
        private readonly YoutubeClient _youtubeClient;

        // fallback when YtExplodeVideoService is broken due to changes on the YT webpage
        private readonly IVideoService _fallback;

        /// <param name="httpClient">The provided HttpClient object needs to have set the user agent string to mimic a desktop browser</param>
        public YtExplodeVideoService(
            HttpClient httpClient,
            ISettings settings,
            ITmdbCachedSearchClient tmdbCachedSearchClient
            ) : base(settings, tmdbCachedSearchClient)
        {
            _youtubeClient = httpClient == null ? new YoutubeClient() : new YoutubeClient(httpClient);
            _fallback = new VanillaYtVideoService(settings, tmdbCachedSearchClient);
        }

        public async Task PopulateWithStreams(TmdbVideoModel attachedVideo)
        {
            try
            {
                MediaStreamInfoSet streamInfoSet = await _youtubeClient.GetVideoMediaStreamInfosAsync(attachedVideo.Key);
                var streams = streamInfoSet.Muxed.Select(stream => GetStreamInfo(stream));

                attachedVideo.Streams = new VideoStreamInfoSet(streams, streamInfoSet.ValidUntil, new YtVideoStreamSelector(_settings));
            } catch { }
        }

        private static VideoStreamInfo GetStreamInfo(MuxedStreamInfo muxedStreamInfo) => new VideoStreamInfo(
            streamUrl: muxedStreamInfo.Url,
            quality: (VideoQuality)muxedStreamInfo.VideoQuality,
            qualityLabel: muxedStreamInfo.VideoQualityLabel,
            height: muxedStreamInfo.Resolution.Height,
            width: muxedStreamInfo.Resolution.Width
            );

        public override async Task PlayVideo(TmdbVideoModel attachedVideo, IPageService pageService)
        {
            if (attachedVideo.Streams == null)
                await PopulateWithStreams(attachedVideo);

            if (attachedVideo.Streams?.SelectedVideoStream != null)
                await pageService.PushVideoPageAsync(attachedVideo);
            else
                await _fallback.PlayVideo(attachedVideo, pageService);
        }
    }

    public class YtVideoStreamSelector : IVideoStreamSelector
    {
        private readonly ISettings _settings;

        public YtVideoStreamSelector(ISettings settings) => _settings = settings;

        public VideoStreamInfo SelectVideoStream(IEnumerable<VideoStreamInfo> streams)
        {
            VideoStreamInfo result = null;

            var orderedByQuality = streams.OrderByDescending(s => s.Quality);

            if (_settings.PlaybackQuality == VideoPlaybackQuality.High)
            {
                result = orderedByQuality.Where(s => s.Quality > VideoQuality.Medium480).LastOrDefault();
                result = result ?? orderedByQuality.FirstOrDefault();
            }
            else if (_settings.PlaybackQuality == VideoPlaybackQuality.Low)
            {
                result = orderedByQuality.Where(s => s.Quality < VideoQuality.High720).FirstOrDefault();
                result = result ?? orderedByQuality.LastOrDefault();
            }            

            return result;
        }
    }
}
#endif
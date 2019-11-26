#if !GOOGLEPLAY
using Ch9.ApiClient;
using Ch9.Models;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Models.MediaStreams;
using System;

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
        // fallback when YtExplodeVideoService is broken due to changes on the YT webpage
        private readonly IVideoService _fallback;

        private readonly YoutubeClient _youtubeClient;
        private readonly IVideoStreamSelector _streamSelector;

        /// <param name="httpClient">The provided HttpClient object needs to have set the user agent string to mimic a desktop browser</param>
        public YtExplodeVideoService(
            HttpClient httpClient,
            ISettings settings,
            ITmdbCachedSearchClient tmdbCachedSearchClient
            ) : base(settings, tmdbCachedSearchClient)
        {
            _fallback = new VanillaYtVideoService(settings, tmdbCachedSearchClient);

            _youtubeClient = httpClient == null ? new YoutubeClient() : new YoutubeClient(httpClient);
            _streamSelector = new YtVideoStreamSelector(settings);
        }

        public async Task PopulateWithStreams(TmdbVideoModel attachedVideo)
        {
            try
            {
                MediaStreamInfoSet streamInfoSet = await _youtubeClient.GetVideoMediaStreamInfosAsync(attachedVideo.Key);
                var streams = streamInfoSet.Muxed.Select(stream => GetStreamInfo(stream));

                attachedVideo.Streams = new VideoStreamInfoSet(streams, streamInfoSet.ValidUntil);
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
            if (attachedVideo.Streams == null || attachedVideo.Streams.ValidUntil < DateTimeOffset.UtcNow)
                await PopulateWithStreams(attachedVideo);

            var selectedStream = attachedVideo.Streams == null ? null : _streamSelector.SelectVideoStream(attachedVideo.Streams.VideoStreams);

            if (selectedStream != null)
                await pageService.PushVideoPageAsync(selectedStream.StreamUrl);
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
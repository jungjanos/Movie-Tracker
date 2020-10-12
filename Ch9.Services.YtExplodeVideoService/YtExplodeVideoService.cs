#if !GOOGLEPLAY

using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using Ch9.Models;
using Ch9.Services.Contracts;

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
            ITmdbApiService tmdbApiService,
            IPlayVideo videoPlayer
            ) : base(settings, tmdbApiService, videoPlayer)
        {
            _fallback = new VanillaYtVideoService(settings, tmdbApiService, _videoPlayer);

            _youtubeClient = httpClient == null ? new YoutubeClient() : new YoutubeClient(httpClient);
            _streamSelector = new YtVideoStreamSelector(settings);
        }

        /// <summary>Tries to populate the Streams property. Does not throw </summary>
        public async Task PopulateWithStreams(TmdbVideoModel attachedVideo)
        {
            try
            {
                var videoManifest = await _youtubeClient.Videos.Streams.GetManifestAsync(attachedVideo.Key);
                var muxedList = videoManifest.GetMuxed();

                var streams = muxedList.Select(stream => MapStreamInfo(stream));

                attachedVideo.Streams = new VideoStreamInfoSet(streams, DateTimeOffset.Now.AddMinutes(60)); // validity no longer provided by YtExplode, setting it arbitrarily 
            }
            catch { }
        }

        private static VideoStreamInfo MapStreamInfo(MuxedStreamInfo muxedStreamInfo) => new VideoStreamInfo(
            streamUrl: muxedStreamInfo.Url,
            quality: (Models.VideoQuality)muxedStreamInfo.VideoQuality,
            qualityLabel: muxedStreamInfo.VideoQualityLabel,
            height: muxedStreamInfo.Resolution.Height,
            width: muxedStreamInfo.Resolution.Width
            );

        public override async Task PlayVideo(TmdbVideoModel attachedVideo)
        {
            if (attachedVideo.Streams == null || attachedVideo.Streams.ValidUntil < DateTimeOffset.UtcNow)
                await PopulateWithStreams(attachedVideo);

            var selectedStream = attachedVideo.Streams == null ? null : _streamSelector.SelectVideoStream(attachedVideo.Streams.VideoStreams);

            if (selectedStream != null)
                await _videoPlayer.OpenVideoStreamDirectly(selectedStream.StreamUrl);
            else
                await _fallback.PlayVideo(attachedVideo);
        }
    }
}
#endif
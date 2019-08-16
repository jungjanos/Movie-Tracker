﻿using Ch9.ApiClient;
using Ch9.Models;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Models;
using YoutubeExplode.Models.MediaStreams;
using Newtonsoft.Json;
using System;
using Ch9.Utils;

namespace Ch9.Services.VideoService
{
    /// <summary>
    /// Video service implementation based on YoutubeExplode. 
    /// Extracts video metadata, thumbnail and stream information without a Youtibe Api key. 
    /// YoutubeExplode is responsible to reverse engineering YT videos from a youtube video-id.
    /// </summary>
    public class YtExplodeVideoService : IVideoService
    {
        private readonly ISettings _settings;
        private readonly ITmdbCachedSearchClient _tmdbCachedSearchClient;
        private readonly YoutubeClient _youtubeClient;

        /// <param name="httpClient">The provided HttpClient object needs to have set the user agent string to mimic a desktop browser</param>
        public YtExplodeVideoService(
            HttpClient httpClient,
            ISettings settings,
            ITmdbCachedSearchClient tmdbCachedSearchClient
            )
        {
            _settings = settings;
            _tmdbCachedSearchClient = tmdbCachedSearchClient;
            _youtubeClient = httpClient == null ? new YoutubeClient() : new YoutubeClient(httpClient);
        }

        public async Task<List<ImageModel>> GetVideoThumbnails(int movieId, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true)
        {
            VideoType typeFilter = VideoType.Trailer | VideoType.Clip | VideoType.BehindTheScene | VideoType.Blooper;

            List<ImageModel> resultingThumbnailsWithoutVideos = new List<ImageModel>();

            GetMovieVideosResult movieVideosResponse = await _tmdbCachedSearchClient.GetMovieVideos(movieId, _settings.SearchLanguage, retryCount, delayMilliseconds, fromCache).ConfigureAwait(false);

            if (movieVideosResponse.HttpStatusCode.IsSuccessCode())
            {
                var tmdbVideosModel = JsonConvert.DeserializeObject<GetMovieVideosModel>(movieVideosResponse.Json);

                foreach (TmdbVideoModel videoModel in tmdbVideosModel.VideoModels)
                {
                    if (string.Equals(videoModel.Site, "YouTube", StringComparison.InvariantCultureIgnoreCase)
                        && ((videoModel.Type & typeFilter) == videoModel.Type) 
                        && YoutubeClient.ValidateVideoId(videoModel.Key))
                    {
                        ImageModel videoThumbnail = new ImageModel
                        {
                            FilePath = new ThumbnailSet(videoModel.Key).HighResUrl,
                            Iso = videoModel.Iso,
                            HasAttachedVideo = true,
                            AttachedVideo = videoModel,
                        };
                        resultingThumbnailsWithoutVideos.Add(videoThumbnail);
                    }
                }
            }
            else
                throw new Exception($"TMDB server responded with {movieVideosResponse.HttpStatusCode}");

            return resultingThumbnailsWithoutVideos;
        }

        public async Task PopulateWithStreams(TmdbVideoModel attachedVideo)
        {
            try
            {
                MediaStreamInfoSet streamInfoSet = await _youtubeClient.GetVideoMediaStreamInfosAsync(attachedVideo.Key);
                var streams = streamInfoSet.Muxed.Select(stream => GetStreamInfo(stream));

                attachedVideo.Streams = new VideoStreamInfoSet(streams, streamInfoSet.ValidUntil, new YtVideoStreamSelector(_settings));
            }
            catch { }
        }

        public static VideoStreamInfo GetStreamInfo(MuxedStreamInfo muxedStreamInfo) => new VideoStreamInfo(
            streamUrl: muxedStreamInfo.Url,
            quality: (VideoQuality)muxedStreamInfo.VideoQuality,
            qualityLabel: muxedStreamInfo.VideoQualityLabel,
            height: muxedStreamInfo.Resolution.Height,
            width: muxedStreamInfo.Resolution.Width
            );
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

            result = result ?? new VideoStreamInfo(streamUrl: string.Empty, VideoQuality.Invalid, "invalid video", -1, -1);

            return result;
        }
    }
}

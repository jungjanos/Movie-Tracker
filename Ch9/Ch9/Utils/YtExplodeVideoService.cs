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

namespace Ch9.Utils
{
    /// <summary>
    /// Video service implementation based on YoutubeExplode. 
    /// Extracts video metadata, thumbnail and stream information without a Youtibe Api key. 
    /// YoutubeExplode is responsible to reverse engineering YT videos from a youtube video-id.
    /// </summary>
    public class YtExplodeVideoService : IVideoService
    {
        private readonly HttpClient _httpClient;
        private readonly ISettings _settings;
        private readonly ITmdbCachedSearchClient _tmdbCachedSearchClient;
        private readonly YoutubeClient _youtubeClient;

        public YtExplodeVideoService(
            HttpClient httpClient,
            ISettings settings,
            ITmdbCachedSearchClient tmdbCachedSearchClient
            )
        {
            _httpClient = httpClient ?? new HttpClient();
            _settings = settings;
            _tmdbCachedSearchClient = tmdbCachedSearchClient;

            // TODO : Raise bugreport: YoutubeExplode: currently can not handle the Android native HttpClient
            _youtubeClient = /*new YoutubeClient(_httpClient)*/ new YoutubeClient();
        }

        public async Task<List<ImageModel>> GetVideoThumbnails(int movieId, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true)
        {
            List<ImageModel> resultingThumbnailsWithoutVideos = new List<ImageModel>();

            GetMovieVideosResult movieVideosResponse = await _tmdbCachedSearchClient.GetMovieVideos(movieId, _settings.SearchLanguage, retryCount, delayMilliseconds, fromCache);

            if (movieVideosResponse.HttpStatusCode.IsSuccessCode())
            {
                JsonConvert.PopulateObject(movieVideosResponse.Json, movieVideosResponse);

                foreach (TmdbVideoModel videoModel in movieVideosResponse.VideoModels)
                {
                    if (string.Equals(videoModel.Site, "YouTube", StringComparison.InvariantCultureIgnoreCase)
                        && string.Equals(videoModel.Type, "Trailer", StringComparison.InvariantCultureIgnoreCase)
                        && YoutubeClient.ValidateVideoId(videoModel.Key))
                    {
                        Video video = null;
                        try
                        {
                            video = await _youtubeClient.GetVideoAsync(videoModel.Key);
                        }
                        catch { }
                        if (video != null)
                        {
                            ImageModel videoThumbnail = new ImageModel
                            {
                                FilePath = video.Thumbnails.MediumResUrl,
                                Iso = videoModel.Iso,
                                HasAttachedVideo = true,
                                AttachedVideo = videoModel,
                            };

                            videoModel.VideoInfo = new VideoInfo(
                                  author: video.Author,
                                  uploadDate: video.UploadDate,
                                  title: video.Title,
                                  description: video.Description,
                                  duration: video.Duration,
                                  statistics: GetStatistics(video)
                                );

                            resultingThumbnailsWithoutVideos.Add(videoThumbnail);
                        }
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

        public static Models.Statistics GetStatistics(Video video) =>  new Models.Statistics(
                 viewCount: video.Statistics.ViewCount,
                 likeCount: video.Statistics.LikeCount,
                 dislikeCount: video.Statistics.DislikeCount,
                 averageRating: video.Statistics.AverageRating
                );

        public static Models.VideoStreamInfo GetStreamInfo(MuxedStreamInfo muxedStreamInfo) => new Models.VideoStreamInfo(
            streamUrl: muxedStreamInfo.Url,
            quality: (Models.VideoQuality)muxedStreamInfo.VideoQuality,
            qualityLabel: muxedStreamInfo.VideoQualityLabel,
            height: muxedStreamInfo.Resolution.Height,
            width:muxedStreamInfo.Resolution.Width
            );
    }

    public class YtVideoStreamSelector : IVideoStreamSelector
    {
        private readonly ISettings _settings;

        public YtVideoStreamSelector(ISettings settings) => _settings = settings;

        public Models.VideoStreamInfo SelectVideoStream(IEnumerable<Models.VideoStreamInfo> streams)
        {
            Models.VideoStreamInfo result = null;

            var orderedByQuality = streams.OrderByDescending(s => s.Quality);

            if (_settings.PlaybackQuality == VideoPlaybackQuality.HighQ)
            {
                result = orderedByQuality.Where(s => s.Quality > Models.VideoQuality.Medium480).LastOrDefault();
                result = result ?? orderedByQuality.FirstOrDefault();
            }
            else if (_settings.PlaybackQuality == VideoPlaybackQuality.LowQ)
            {
                result = orderedByQuality.Where(s => s.Quality < Models.VideoQuality.High720).FirstOrDefault();
                result = result ?? orderedByQuality.LastOrDefault();
            }

            result = result ?? new Models.VideoStreamInfo(streamUrl: string.Empty, Models.VideoQuality.Invalid, "invalid video", -1, -1);

            return result;
        }
    }
}
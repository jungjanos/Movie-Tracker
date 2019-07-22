using Ch9.ApiClient;
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

            _youtubeClient = /*new YoutubeClient(_httpClient)*/ new YoutubeClient();
        }

        public async Task<List<ImageModel>> GetVideoThumbnailsWithVideoStreams(int movieId, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true)
        {
            List<ImageModel> resultingThumbnailsWithVideos = new List<ImageModel>();
            GetMovieVideosResult movieVideosResponse = await _tmdbCachedSearchClient.GetMovieVideos(movieId, _settings.SearchLanguage, retryCount, delayMilliseconds, fromCache);

            if (movieVideosResponse.HttpStatusCode.IsSuccessCode())
            {
                JsonConvert.PopulateObject(movieVideosResponse.Json, movieVideosResponse);

                foreach (TmdbVideoModel videoModel in movieVideosResponse.VideoModels)
                {
                    if (videoModel.Site == "YouTube" && YoutubeClient.ValidateVideoId(videoModel.Key))
                    {
                        try
                        {
                            videoModel.VideoInfo = await GetVideoInfo(videoModel.Key);
                        } catch { }
                    }
                }                

                foreach (TmdbVideoModel videoModel in movieVideosResponse.VideoModels)
                {
                    if (videoModel?.VideoInfo?.SelectedStream != null)
                    {
                        ImageModel videoThumbnail = new ImageModel
                        {
                            FilePath = videoModel.VideoInfo.ThumbnailMediumRes.FilePath,
                            Iso = videoModel.Iso,
                            AttachedVideo = videoModel,
                        };
                        resultingThumbnailsWithVideos.Add(videoThumbnail);
                    }
                }
            }
            return resultingThumbnailsWithVideos;
        }

        private async Task<VideoInfo> GetVideoInfo(string id)
        {
            Video video = await _youtubeClient.GetVideoAsync(id);

            MediaStreamInfoSet streamInfoSet = await _youtubeClient.GetVideoMediaStreamInfosAsync(id);

            List<Models.VideoStreamInfo> videoStreams = new List<Models.VideoStreamInfo>();

            foreach (MuxedStreamInfo stream in streamInfoSet.Muxed)
            {
                Models.VideoStreamInfo videoStreamInfo = new Models.VideoStreamInfo(
                    streamUrl: stream.Url,
                    quality: (Models.VideoQuality)stream.VideoQuality,
                    qualityLabel: stream.VideoQualityLabel,
                    height: stream.Resolution.Height,
                    width: stream.Resolution.Width
                    );

                videoStreams.Add(videoStreamInfo);
            }

            return new VideoInfo(
                id: id,
                author: video.Author,
                uploadDate: video.UploadDate,
                title: video.Title,
                description: video.Description,
                thumbnailHighRes: video.Thumbnails.HighResUrl,
                thumbnailMediumRes: video.Thumbnails.MediumResUrl,
                video.Duration, video.Keywords,
                new Models.Statistics(video.Statistics.ViewCount, video.Statistics.LikeCount, video.Statistics.DislikeCount),
                videoStreams,
                new YtVideoQualitySelector(_settings));
        }
    }

    public class YtVideoQualitySelector : IVideoQualitySelector
    {
        private readonly ISettings _settings;

        public YtVideoQualitySelector(ISettings settings) => _settings = settings;

        public Models.VideoStreamInfo SelectVideoStream(IList<Models.VideoStreamInfo> streams)
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

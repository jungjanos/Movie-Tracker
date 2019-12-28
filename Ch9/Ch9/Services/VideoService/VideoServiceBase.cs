using Ch9.Infrastructure.Extensions;
using Ch9.Services.Contracts;
using Ch9.Ui.Contracts.Models;

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ch9.Services.VideoService
{
    /// <summary>
    /// Base class for video service implementations.
    /// </summary>
    public abstract class VideoServiceBase : IVideoService
    {
        protected readonly ISettings _settings;
        protected readonly ITmdbApiService _tmdbApiService;
        private readonly StringToVideoTypeConverter _converter = new StringToVideoTypeConverter();

        public VideoServiceBase(
            ISettings settings,
            ITmdbApiService tmdbApiService)
        {
            _settings = settings;
            _tmdbApiService = tmdbApiService;
        }
        /// <summary>
        /// Gets list of associated video entries from TMDb for the given Movie
        /// </summary>
        /// <param name="movieId">TMDb movie id</param>
        /// <returns>List of thumbnails each with an attached video object</returns>
        public virtual async Task<List<ImageModel>> GetVideoThumbnails(int movieId, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true)
        {
            VideoType typeFilter = _settings.PreferredVideoTypes;

            List<ImageModel> resultingThumbnailsWithoutVideos = new List<ImageModel>();

            var response = await _tmdbApiService.TryGetMovieVideos(movieId, _settings.SearchLanguage, retryCount, delayMilliseconds, fromCache).ConfigureAwait(false);

            if (response.HttpStatusCode.IsSuccessCode())
            {
                var movieVideos = response.MovieVideosModel;

                foreach (TmdbVideoModel videoModel in movieVideos.VideoModels)
                {
                    var videoType = _converter.Convert(videoModel.TypeStr);

                    if (string.Equals(videoModel.Site, "YouTube", StringComparison.InvariantCultureIgnoreCase)
                        && ((videoType & typeFilter) == videoType)
                        && ValidateVideoId(videoModel.Key)
                        )
                    {
                        ImageModel videoThumbnail = new ImageModel
                        {
                            FilePath = GetThumbnailFilePath(videoModel.Key),
                            Iso = videoModel.Iso,
                            HasAttachedVideo = true,
                            AttachedVideo = videoModel,
                        };
                        resultingThumbnailsWithoutVideos.Add(videoThumbnail);
                    }
                }
            }
            else
                throw new Exception($"TMDB server responded with {response.HttpStatusCode}");

            return resultingThumbnailsWithoutVideos;
        }

        protected virtual bool ValidateVideoId(string id)
        {
            // currently only for YT
            // length is not checked (as of 2019.08.23, the length of a YT-id is always 11 chars)
            // as this will break the function if Youtube changes
            return !Regex.IsMatch(id, @"[^0-9a-zA-Z_\-]", RegexOptions.Compiled);
        }

        protected virtual string GetThumbnailFilePath(string id) => $"https://img.youtube.com/vi/{id}/hqdefault.jpg";
        public abstract Task PlayVideo(TmdbVideoModel attachedVideo, IPageService pageService);
    }
}

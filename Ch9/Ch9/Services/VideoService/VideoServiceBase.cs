using Ch9.ApiClient;
using Ch9.Models;
using Ch9.Utils;
using Newtonsoft.Json;
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
        protected readonly ITmdbCachedSearchClient _tmdbCachedSearchClient;
        
        public VideoServiceBase(
            ISettings settings,
            ITmdbCachedSearchClient tmdbCachedSearchClient)
        {
            _settings = settings;
            _tmdbCachedSearchClient = tmdbCachedSearchClient;            
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

            GetMovieVideosResult movieVideosResponse = await _tmdbCachedSearchClient.GetMovieVideos(movieId, _settings.SearchLanguage, retryCount, delayMilliseconds, fromCache).ConfigureAwait(false);

            if (movieVideosResponse.HttpStatusCode.IsSuccessCode())
            {
                var tmdbVideosModel = JsonConvert.DeserializeObject<GetMovieVideosModel>(movieVideosResponse.Json);

                foreach (TmdbVideoModel videoModel in tmdbVideosModel.VideoModels)
                {
                    if (string.Equals(videoModel.Site, "YouTube", StringComparison.InvariantCultureIgnoreCase)
                        && ((videoModel.Type & typeFilter) == videoModel.Type)
                        && ValidateVideoId(videoModel.Key))
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
                throw new Exception($"TMDB server responded with {movieVideosResponse.HttpStatusCode}");

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

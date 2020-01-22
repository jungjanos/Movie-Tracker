using Ch9.Infrastructure.Extensions;
using Ch9.Models;
using Ch9.Services.Contracts;
using Ch9.Services.VideoService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace Ch9.Services
{
    public class MovieDetailsService : IMovieDetailsService
    {
        private readonly ISettings _settings;
        private readonly ITmdbApiService _tmdbApiService;
        private readonly IMovieDetailModelConfigurator _movieDetailModelConfigurator;
        private readonly IPersonDetailModelConfigurator _personDetailModelConfigurator;
        private readonly IVideoService _videoService;

        public MovieDetailsService(
            ISettings settings,
            ITmdbApiService tmdbApiService,
            IMovieDetailModelConfigurator movieDetailModelConfigurator,
            IPersonDetailModelConfigurator personDetailModelConfigurator,
            IVideoService videoService)
        {
            _settings = settings;
            _tmdbApiService = tmdbApiService;
            _movieDetailModelConfigurator = movieDetailModelConfigurator;
            _personDetailModelConfigurator = personDetailModelConfigurator;
            _videoService = videoService;
        }

        /// <summary>
        /// Fetches gallery image Url-s and populates the movie object with it
        /// </summary>        
        public async Task LoadMovieGallery(MovieDetailModel movieToPopulate, int retryCount, int delayMilliseconds, bool fromCache)
        {
            var response = await _tmdbApiService.TryGetMovieImages(movieToPopulate.Id, _settings.SearchLanguage, otherLanguage: null, includeLanguageless: true, retryCount: retryCount, delayMilliseconds: delayMilliseconds, fromCache: true);

            if (response.HttpStatusCode.IsSuccessCode())
            {
                movieToPopulate.ImageDetailCollection = response.ImageDetailCollection;
                _movieDetailModelConfigurator.SetGalleryImageSources(movieToPopulate);
            }
        }

        /// <summary>
        /// Fetches video clip thumbnail data and populates the movie object with it
        /// </summary>        
        public async Task LoadVideoThumbnailCollection(MovieDetailModel movieToPopulate, int retryCount, int delayMilliseconds, bool fromCache)
        {
            if (movieToPopulate.VideoThumbnails != null)
                return;

            movieToPopulate.VideoThumbnails = new ObservableCollection<ImageModel>();

            List<ImageModel> videoThumbnails = await _videoService.GetVideoThumbnails(movieToPopulate.Id, retryCount: retryCount, delayMilliseconds: delayMilliseconds, fromCache: true);
            foreach (var thumbnail in videoThumbnails)
                movieToPopulate.VideoThumbnails.Add(thumbnail);
        }

        public async Task<List<IStaffMemberRole>> FetchMovieCredits(int movieId, int retryCount, int delayMilliseconds, bool fromCache)
        {
            var response = await _tmdbApiService.TryGetMovieCredits(movieId, retryCount: retryCount, delayMilliseconds: delayMilliseconds, fromCache: fromCache);

            if (response.HttpStatusCode.IsSuccessCode())
            {
                var result = response.MovieCreditsModel.ExtractStaffToDisplay(7);                
                _personDetailModelConfigurator.SetProfileImageSrc(result);

                return result;
            }
            else 
                throw new Exception($"Could not load the credits list, service responded with: {response.HttpStatusCode}");
        }
    }
}

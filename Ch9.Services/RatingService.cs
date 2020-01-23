using Ch9.Models;
using Ch9.Services.Contracts;
using Ch9.Infrastructure.Extensions;

using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Ch9.Services
{
    public class RatingService : IRatingService
    {
        private readonly ISettings _settings;
        private readonly ITmdbApiService _tmdbApiService;

        public RatingService(ISettings settings, ITmdbApiService tmdbApiService)
        {
            _settings = settings;
            _tmdbApiService = tmdbApiService;
        }

        public async Task DeleteRating(int movieId, int retryCount, int delayMilliseconds)
        {
            if (!_settings.IsLoggedin)
                throw new Exception($"Not logged in");

            var response = await _tmdbApiService.TryDeleteMovieRating(movieId, null, retryCount: retryCount, delayMilliseconds: delayMilliseconds);

            if (!response.HttpStatusCode.IsSuccessCode())
                throw new Exception($"Could not delete your rating, server reponse: {response.HttpStatusCode}");
        }

        public async Task SetRating(decimal rating, int movieId, int retryCount, int delayMilliseconds)
        {
            if (!_settings.IsLoggedin)
                throw new Exception($"Not logged in");

            var response = await _tmdbApiService.TryRateMovie(rating: rating, mediaId: movieId, guestSessionId: null, retryCount: retryCount, delayMilliseconds: delayMilliseconds);

            if (!response.HttpStatusCode.IsSuccessCode())
                throw new Exception($"Could not post your rating, server reponse: {response.HttpStatusCode}");
        }

        public async Task<List<Review>> TryFetchMovieReviewsFirstPage(int movieId, int retryCount, int delayMilliseconds)
        {
            List<Review> result = null;

            try
            {
                var response = await _tmdbApiService.TryGetMovieReviews(movieId, language: null, page: null, retryCount: retryCount, delayMilliseconds: delayMilliseconds, fromCache: true);
                if (response.HttpStatusCode.IsSuccessCode())
                {
                    ReviewsModel reviewsModel = response.ReviewsModel;
                    result = new List<Review>(reviewsModel.Reviews);
                }
            }
            catch { }

            return result ?? new List<Review>();
        }

    }
}

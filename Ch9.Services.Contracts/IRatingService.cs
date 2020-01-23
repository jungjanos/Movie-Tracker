using Ch9.Models;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ch9.Services.Contracts
{
    public interface IRatingService
    {
        Task DeleteRating(int movieId, int retryCount, int delayMilliseconds);
        Task SetRating(decimal rating, int movieId, int retryCount, int delayMilliseconds);
        Task<List<Review>> TryFetchMovieReviewsFirstPage(int movieId, int retryCount, int delayMilliseconds);
    }
}
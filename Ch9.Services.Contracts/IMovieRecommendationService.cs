using Ch9.Models;

using System.Threading.Tasks;

namespace Ch9.Services.Contracts
{
    public interface IMovieRecommendationService
    {
        Task<SearchResult> LoadMovieRecommendationsPage(int movieId, int page, int retryCount, int delayMilliseconds);
        Task<SearchResult> LoadSimilarMoviesPage(int movieId, int page, int retryCount, int delayMilliseconds);
    }
}
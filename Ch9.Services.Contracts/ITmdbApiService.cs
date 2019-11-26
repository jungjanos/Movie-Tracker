using System.Net;
using System.Threading.Tasks;

namespace Ch9.Services.Contracts
{
    public interface ITmdbApiService
    {
        string SessionId { get; set; }

        Task<TryGetMovieCreditsResponse> TryGetMovieCredits(int id, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true);
        Task<HttpStatusCode> TryGetMovieDetails(int id, string language = null, int retryCount = 0, int delayMilliseconds = 1000);
        Task<TryGetMovieImagesResponse> TryGetMovieImages(int id, string language = null, string otherLanguage = null, bool? includeLanguageless = true, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true);
        Task<TryGetPersonsDetailsResponse> TryGetPersonsDetails(int personId, string language = null, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true);
    }
}

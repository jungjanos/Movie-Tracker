using System.Threading.Tasks;

namespace Ch9.ApiClient
{
    public interface ITmdbNetworkClient
    {
        Task<GenreNameFetchResult> FetchGenreIdsWithNames(string language = null, int retryCount = 0, int delayMilliseconds = 1000);
        Task<FetchMovieDetailsResult> FetchMovieDetails(int id, string language = null, int retryCount = 0, int delayMilliseconds = 1000);
        Task<GetMovieRecommendationsResult> GetMovieRecommendations(int id, string language = null, int retryCount = 0, int delayMilliseconds = 1000);
        Task<GetSimilarMoviesResult> GetSimilarMovies(int id, string language = null, int retryCount = 0, int delayMilliseconds = 1000);
        Task<TmdbConfigurationModelResult> GetTmdbConfiguration(int retryCount = 0, int delayMilliseconds = 1000);
        Task<TrendingMoviesResult> GetTrendingMovies(bool week = true, string language = null, bool? includeAdult = null, int? page = null, int retryCount = 0, int delayMilliseconds = 1000);
        Task<SearchByMovieResult> SearchByMovie(string searchString, string language = null, bool? includeAdult = null, int? page = null, int retryCount = 0, int delayMilliseconds = 1000);
        Task<GetMovieImagesResult> UpdateMovieImages(int id, string language = null, string otherLanguage = null, bool? includeLanguageless = true, int retryCount = 0, int delayMilliseconds = 1000);
    }
}
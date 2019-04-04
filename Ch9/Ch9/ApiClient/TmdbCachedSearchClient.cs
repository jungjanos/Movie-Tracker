using LazyCache;
using System.Threading.Tasks;

namespace Ch9.ApiClient
{
    public class TmdbCachedSearchClient
    {
        private IAppCache _cache;
        private ITmdbNetworkClient _networkClient;

        public TmdbCachedSearchClient(ITmdbNetworkClient theMovieDatabaseClient)
        {
            _cache = new CachingService();
            _networkClient = theMovieDatabaseClient;
        }

        #region CachedQueries
        public async Task<SearchByMovieResult> SearchByMovie(string searchString, string language = null, bool? includeAdult = null, int? page = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            return await _cache.GetOrAddAsync("$" + nameof(SearchByMovie) + searchString + (language ?? "") + 
                (includeAdult?.ToString() ?? ""), () => _networkClient.SearchByMovie(searchString, language, includeAdult));            
        }
        
        public async Task<FetchMovieDetailsResult> FetchMovieDetails(int id, string language = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            return await _cache.GetOrAddAsync("$" + nameof(FetchMovieDetails)  + id.ToString() + (language ?? ""), () => _networkClient.FetchMovieDetails(id, language));            
        }
        
        public async Task<TrendingMoviesResult> GetTrendingMovies(bool week = true, string language = null, bool? includeAdult = null, int? page = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            return await _cache.GetOrAddAsync("$" + nameof(GetTrendingMovies) + week + (language ?? "") + (includeAdult?.ToString() ?? "") + (page?.ToString() ?? ""), () =>
               _networkClient.GetTrendingMovies(week, language, includeAdult));
        }
        
        public async Task<GetMovieImagesResult> UpdateMovieImages(int id, string language = null, string otherLanguage = null, bool? includeLanguageless = true, int retryCount = 0, int delayMilliseconds = 1000)
        {
            return await _cache.GetOrAddAsync("$" + nameof(UpdateMovieImages) + id.ToString() + (language ?? "") + (otherLanguage ?? "") + (includeLanguageless == null ? "" : includeLanguageless.Value.ToString()),

                    () => _networkClient.UpdateMovieImages(id, language, otherLanguage, includeLanguageless));
        }

        public async Task<GetMovieRecommendationsResult> GetMovieRecommendations(int id, string language = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            return await _cache.GetOrAddAsync("$" + nameof(GetMovieRecommendations) + id.ToString() + (language ?? ""), 

                    () => _networkClient.GetMovieRecommendations(id, language, retryCount, delayMilliseconds));
        }

        public async Task<GetSimilarMoviesResult> GetSimilarMovies(int id, string language = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            return await _cache.GetOrAddAsync("$" + nameof(GetSimilarMovies) + id.ToString() + (language ?? ""),

                    () => _networkClient.GetSimilarMovies(id, language, retryCount, delayMilliseconds));
        }
        #endregion

        #region UncachedQueries
        public async Task<GenreNameFetchResult> FetchGenreIdsWithNames(string language = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            return await _networkClient.FetchGenreIdsWithNames(language, retryCount, delayMilliseconds);
        }

        public async Task<TmdbConfigurationModelResult> GetTmdbConfiguration(int retryCount = 0, int delayMilliseconds = 1000)
        {
            return await _networkClient.GetTmdbConfiguration(retryCount, delayMilliseconds);
        }

        #endregion
    }
}

using Ch9.Utils;
using LazyCache;
using System.Threading.Tasks;

namespace Ch9.ApiClient
{    
    // TODO : evaluate whether to remove empty async-await from calls
    // remark: whitepaper of Cleary
    public class TmdbCachedSearchClient : ITmdbCachedSearchClient
    {
        private readonly IAppCache _cache;
        private readonly ITmdbNetworkClient _networkClient;

        public TmdbCachedSearchClient(ITmdbNetworkClient theMovieDatabaseClient)
        {
            _cache = new CachingService();
            _networkClient = theMovieDatabaseClient;
        }

        #region CachedQueries
        public async Task<TmdbConfigurationModelResult> GetTmdbConfiguration(int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true)
        {
            string key = "$" + nameof(GetTmdbConfiguration);

            if (!fromCache)
                _cache.Remove(key);

            var result = _cache.Get<TmdbConfigurationModelResult>(key) ?? await _networkClient.GetTmdbConfiguration(retryCount, delayMilliseconds);

            if (result.HttpStatusCode.IsSuccessCode())
                _cache.Add(key, result, System.TimeSpan.FromDays(1));

            return result;

            //return await _networkClient.GetTmdbConfiguration(retryCount, delayMilliseconds);
        }
        public async Task<SearchByMovieResult> SearchByMovie(string searchString, string language = null, bool? includeAdult = null, int? page = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            string key = "$" + nameof(SearchByMovie) + searchString + (language ?? "") + (includeAdult?.ToString() ?? "");

            var result =  _cache.Get<SearchByMovieResult>(key) ?? await _networkClient.SearchByMovie(searchString, language, includeAdult, page, retryCount, delayMilliseconds);

            if (result.HttpStatusCode.IsSuccessCode())
                _cache.Add(key, result);

            return result;
        }

        public async Task<FetchMovieDetailsResult> FetchMovieDetails(int id, string language = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            string key = "$" + nameof(FetchMovieDetails) + id.ToString() + (language ?? "");

            var result = _cache.Get<FetchMovieDetailsResult>(key) ?? await _networkClient.FetchMovieDetails(id, language, retryCount, delayMilliseconds);

            if (result.HttpStatusCode.IsSuccessCode())
                _cache.Add(key, result);

            return result;
        }

        public async Task<TrendingMoviesResult> GetTrendingMovies(bool week = true, string language = null, bool? includeAdult = null, int? page = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            string key = "$" + nameof(GetTrendingMovies) + week + (language ?? "") + (includeAdult?.ToString() ?? "") + (page?.ToString() ?? "");

            var result = _cache.Get<TrendingMoviesResult>(key) ?? await _networkClient.GetTrendingMovies(week, language, includeAdult, page, retryCount, delayMilliseconds);

            if (result.HttpStatusCode.IsSuccessCode())
                _cache.Add(key, result);

            return result;
        }

        public async Task<GetMovieImagesResult> UpdateMovieImages(int id, string language = null, string otherLanguage = null, bool? includeLanguageless = true, int retryCount = 0, int delayMilliseconds = 1000)
        {
            string key = "$" + nameof(UpdateMovieImages) + id + (language ?? "") + (otherLanguage ?? "") + (includeLanguageless == null ? "" : includeLanguageless.Value.ToString());

            var result = _cache.Get<GetMovieImagesResult>(key) ?? await _networkClient.UpdateMovieImages(id, language, otherLanguage, includeLanguageless, retryCount, delayMilliseconds);

            if (result.HttpStatusCode.IsSuccessCode())
                _cache.Add(key, result);

            return result;
        }

        public async Task<GetMovieRecommendationsResult> GetMovieRecommendations(int id, string language = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            string key = "$" + nameof(GetMovieRecommendations) + id + (language ?? "");

            var result = _cache.Get<GetMovieRecommendationsResult>(key) ?? await _networkClient.GetMovieRecommendations(id, language, retryCount, delayMilliseconds);

            if (result.HttpStatusCode.IsSuccessCode())
                _cache.Add(key, result);

            return result;
        }

        public async Task<GetSimilarMoviesResult> GetSimilarMovies(int id, string language = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            string key = "$" + nameof(GetSimilarMovies) + id + (language ?? "");

            var result = _cache.Get<GetSimilarMoviesResult>(key) ?? await _networkClient.GetSimilarMovies(id, language, retryCount, delayMilliseconds);

            if (result.HttpStatusCode.IsSuccessCode())
                _cache.Add(key, result);

            return result;
        }

        public async Task<GetListsResult> GetLists(int? accountId = null, string language = null, int? page = null, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true)
        {
            string key = "$" + nameof(GetLists) + (accountId?.ToString() ?? "") + (language ?? "") + (page?.ToString() ?? "");

            if (!fromCache)
                _cache.Remove(key);

            var result = _cache.Get<GetListsResult>(key) ?? await _networkClient.GetLists(accountId, language, page, retryCount, delayMilliseconds);

            if (result.HttpStatusCode.IsSuccessCode())
                _cache.Add(key, result);

            return result;
        }

        public async Task<GetListDetailsResult> GetListDetails(int listId, string language = null, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true)
        {
            string key = "$" + nameof(GetListDetails) + listId + (language ?? "");

            if (!fromCache)
                _cache.Remove(key);

            var result = _cache.Get<GetListDetailsResult>(key) ?? await _networkClient.GetListDetails(listId, language, retryCount, delayMilliseconds);

            if (result.HttpStatusCode.IsSuccessCode())
                _cache.Add(key, result);

            return result;
        }

        public async Task<GetMovieReviewsResult> GetMovieReviews(int id, string language = null, int? page = null, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true)
        {
            string key = "$" + nameof(GetMovieReviews) + id + (language ?? "") + (page?.ToString() ?? "");

            if (!fromCache)
                _cache.Remove(key);

            var result = _cache.Get<GetMovieReviewsResult>(key) ?? await _networkClient.GetMovieReviews(id, language, page, retryCount, delayMilliseconds);

            if (result.HttpStatusCode.IsSuccessCode())
                _cache.Add(key, result);

            return result;
        }

        #endregion

        #region UncachedQueries
        public async Task<GenreNameFetchResult> FetchGenreIdsWithNames(string language = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            return await _networkClient.FetchGenreIdsWithNames(language, retryCount, delayMilliseconds);
        }

        public async Task<AddMovieResult> AddMovie(int listId, int mediaId, int retryCount = 0, int delayMilliseconds = 1000)
        {
            return await _networkClient.AddMovie(listId, mediaId, retryCount, delayMilliseconds);
        }

        public async Task<RemoveMovieResult> RemoveMovie(int listId, int mediaId, int retryCount = 0, int delayMilliseconds = 1000)
        {
            return await _networkClient.RemoveMovie(listId, mediaId, retryCount, delayMilliseconds);
        }

        public async Task<DeleteListResult> DeleteList(int listId, int retryCount = 0, int delayMilliseconds = 1000)
        {
            return await _networkClient.DeleteList(listId, retryCount, delayMilliseconds);
        }

        public async Task<CreateListResult> CreateList(string name, string description, string language = "en", int retryCount = 0, int delayMilliseconds = 1000)
        {
            return await _networkClient.CreateList(name, description, language, retryCount, delayMilliseconds);
        }

        public async Task<GetAccountMovieStatesResult> GetAccountMovieStates(int mediaId, string guestSessionId = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            return await _networkClient.GetAccountMovieStates(mediaId, guestSessionId, retryCount, delayMilliseconds);
        }

        public async Task<UpdateFavoriteListResult> UpdateFavoriteList(string mediaType, bool add, int mediaId, int? accountId = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            return await _networkClient.UpdateFavoriteList(mediaType, add, mediaId, accountId, retryCount, delayMilliseconds);
        }

        public async Task<UpdateWatchlistResult> UpdateWatchlist(string mediaType, bool add, int mediaId, int? accountId = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            return await _networkClient.UpdateWatchlist(mediaType, add, mediaId, accountId, retryCount, delayMilliseconds);
        }

        public async Task<RateMovieResult> RateMovie(Rating rating, int mediaId, string guestSessionId = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            return await _networkClient.RateMovie(rating, mediaId, guestSessionId, retryCount, delayMilliseconds);
        }

        public async Task<DeleteMovieRatingResult> DeleteMovieRating(int mediaId, string guestSessionId = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            return await _networkClient.DeleteMovieRating(mediaId, guestSessionId, retryCount, delayMilliseconds);
        }

        #endregion
    }
}

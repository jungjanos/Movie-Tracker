using Ch9.Data.Contracts;
using Ch9.Infrastructure.Extensions;
using LazyCache;
using System.Threading.Tasks;

namespace Ch9.Data.ApiClient
{
    // TODO : evaluate whether to remove empty async-await from calls
    // remark: whitepaper of Cleary
    public class TmdbCachedSearchClient : ITmdbCachedSearchClient
    {
        private readonly IAppCache _cache;
        private readonly ITmdbNetworkClient _networkClient;

        public string SessionId
        {
            get => _networkClient.SessionId;
            set => _networkClient.SessionId = value;
        }

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
        public async Task<SearchByMovieResult> SearchByMovie(string searchString, string language = null, bool? includeAdult = null, int? page = null, int? year = null, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true)
        {
            string key = "$" + nameof(SearchByMovie) + searchString + (language ?? "") + (includeAdult?.ToString() ?? "") + (page?.ToString() ?? "") + (year?.ToString() ?? "");

            if (!fromCache)
                _cache.Remove(key);

            var result = _cache.Get<SearchByMovieResult>(key) ?? await _networkClient.SearchByMovie(searchString, language, includeAdult, page, year, retryCount, delayMilliseconds);

            if (result.HttpStatusCode.IsSuccessCode())
                _cache.Add(key, result);

            return result;
        }

        public async Task<GetMovieDetailsResult> GetMovieDetails(int id, string language = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            string key = "$" + nameof(GetMovieDetails) + id.ToString() + (language ?? "");

            var result = _cache.Get<GetMovieDetailsResult>(key) ?? await _networkClient.GetMovieDetails(id, language, retryCount, delayMilliseconds);

            if (result.HttpStatusCode.IsSuccessCode())
                _cache.Add(key, result);

            return result;
        }

        public async Task<TrendingMoviesResult> GetTrendingMovies(bool week = true, string language = null, bool? includeAdult = null, int? page = null, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true)
        {
            string key = "$" + nameof(GetTrendingMovies) + week + (language ?? "") + (includeAdult?.ToString() ?? "") + (page?.ToString() ?? "");

            if (!fromCache)
                _cache.Remove(key);

            var result = _cache.Get<TrendingMoviesResult>(key) ?? await _networkClient.GetTrendingMovies(week, language, includeAdult, page, retryCount, delayMilliseconds);

            if (result.HttpStatusCode.IsSuccessCode())
                _cache.Add(key, result);

            return result;
        }

        public async Task<GetMovieImagesResult> GetMovieImages(int id, string language = null, string otherLanguage = null, bool? includeLanguageless = true, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true)
        {
            string key = "$" + nameof(GetMovieImages) + id + (language ?? "") + (otherLanguage ?? "") + (includeLanguageless == null ? "" : includeLanguageless.Value.ToString());

            if (!fromCache)
                _cache.Remove(key);

            var result = _cache.Get<GetMovieImagesResult>(key) ?? await _networkClient.GetMovieImages(id, language, otherLanguage, includeLanguageless, retryCount, delayMilliseconds);

            if (result.HttpStatusCode.IsSuccessCode())
                _cache.Add(key, result);

            return result;
        }

        public async Task<GetPersonImagesResult> GetPersonImages(int id, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true)
        {
            string key = "$" + nameof(GetPersonImages) + id;

            if (!fromCache)
                _cache.Remove(key);

            var result = _cache.Get<GetPersonImagesResult>(key) ?? await _networkClient.GetPersonImages(id, retryCount, delayMilliseconds);

            if (result.HttpStatusCode.IsSuccessCode())
                _cache.Add(key, result);

            return result;
        }

        public async Task<GetMovieRecommendationsResult> GetMovieRecommendations(int id, string language = null, int? page = null, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true)
        {
            string key = "$" + nameof(GetMovieRecommendations) + id + (language ?? "") + (page?.ToString() ?? "");

            if (!fromCache)
                _cache.Remove(key);

            var result = _cache.Get<GetMovieRecommendationsResult>(key) ?? await _networkClient.GetMovieRecommendations(id, language, page, retryCount, delayMilliseconds);

            if (result.HttpStatusCode.IsSuccessCode())
                _cache.Add(key, result);

            return result;
        }

        public async Task<GetSimilarMoviesResult> GetSimilarMovies(int id, string language = null, int? page = null, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true)
        {
            string key = "$" + nameof(GetSimilarMovies) + id + (language ?? "") + (page?.ToString() ?? "");

            if (!fromCache)
                _cache.Remove(key);

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

        public async Task<GetMovieCreditsResult> GetMovieCredits(int id, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true)
        {
            string key = "$" + nameof(GetMovieCredits) + id;

            if (!fromCache)
                _cache.Remove(key);

            var result = _cache.Get<GetMovieCreditsResult>(key) ?? await _networkClient.GetMovieCredits(id, retryCount, delayMilliseconds);

            if (result.HttpStatusCode.IsSuccessCode())
                _cache.Add(key, result);

            return result;
        }

        public async Task<GetPersonsMovieCreditsResult> GetPersonsMovieCredits(int personId, string language = null, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true)
        {
            string key = "$" + nameof(GetPersonsMovieCredits) + personId + (language ?? "");

            if (!fromCache)
                _cache.Remove(key);

            var result = _cache.Get<GetPersonsMovieCreditsResult>(key) ?? await _networkClient.GetPersonsMovieCredits(personId, language, retryCount, delayMilliseconds);

            if (result.HttpStatusCode.IsSuccessCode())
                _cache.Add(key, result);

            return result;
        }

        public async Task<GetPersonsDetailsResult> GetPersonsDetails(int personId, string language = null, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true)
        {
            string key = "$" + nameof(GetPersonsDetails) + personId + (language ?? "");

            if (!fromCache)
                _cache.Remove(key);

            var result = _cache.Get<GetPersonsDetailsResult>(key) ?? await _networkClient.GetPersonsDetails(personId, language, retryCount, delayMilliseconds);

            if (result.HttpStatusCode.IsSuccessCode())
                _cache.Add(key, result);

            return result;
        }

        public async Task<GetMovieVideosResult> GetMovieVideos(int id, string language = null, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true)
        {
            string key = "$" + nameof(GetMovieVideos) + id + (language ?? "");

            if (!fromCache)
                _cache.Remove(key);

            var result = _cache.Get<GetMovieVideosResult>(key) ?? await _networkClient.GetMovieVideos(id, language, retryCount, delayMilliseconds);

            if (result.HttpStatusCode.IsSuccessCode())
                _cache.Add(key, result);

            return result;
        }
        #endregion

        #region UncachedQueries
        public async Task<GenreNameFetchResult> GetGenreIdsWithNames(string language = null, int retryCount = 0, int delayMilliseconds = 1000)
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

        public async Task<GetItemStatusOnTargetListResult> GetItemStatusOnTargetList(int listId, int movieId, int retryCount = 0, int delayMilliseconds = 1000)
        {
            return await _networkClient.GetItemStatusOnTargetList(listId, movieId, retryCount, delayMilliseconds);
        }

        public async Task<UpdateFavoriteListResult> UpdateFavoriteList(string mediaType, bool add, int mediaId, int? accountId = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            return await _networkClient.UpdateFavoriteList(mediaType, add, mediaId, accountId, retryCount, delayMilliseconds);
        }

        public async Task<UpdateWatchlistResult> UpdateWatchlist(string mediaType, bool add, int mediaId, int? accountId = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            return await _networkClient.UpdateWatchlist(mediaType, add, mediaId, accountId, retryCount, delayMilliseconds);
        }

        public async Task<RateMovieResult> RateMovie(decimal rating, int mediaId, string guestSessionId = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            return await _networkClient.RateMovie(rating, mediaId, guestSessionId, retryCount, delayMilliseconds);
        }

        public async Task<DeleteMovieRatingResult> DeleteMovieRating(int mediaId, string guestSessionId = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            return await _networkClient.DeleteMovieRating(mediaId, guestSessionId, retryCount, delayMilliseconds);
        }

        public async Task<GetFavoriteMoviesResult> GetFavoriteMovies(int? accountId = null, string language = null, string sortBy = null, int? page = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            return await _networkClient.GetFavoriteMovies(accountId, language, sortBy, page, retryCount, delayMilliseconds);
        }

        public async Task<GetMovieWatchlistResult> GetMovieWatchlist(int? accountId = null, string language = null, string sortBy = null, int? page = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            return await _networkClient.GetMovieWatchlist(accountId, language, sortBy, page, retryCount, delayMilliseconds);
        }

        public async Task<GetMovieDetailsWithAccountStatesResult> GetMovieDetailsWithAccountStates(int id, string language = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            return await _networkClient.GetMovieDetailsWithAccountStates(id, language, retryCount, delayMilliseconds);
        }

        public async Task<DeleteSessionResult> DeleteSession(string sessionId, int retryCount = 0, int delayMilliseconds = 1000)
        {
            return await _networkClient.DeleteSession(sessionId, retryCount, delayMilliseconds);
        }

        public async Task<CreateRequestTokenResult> CreateRequestToken(int retryCount = 0, int delayMilliseconds = 1000)
        {
            return await _networkClient.CreateRequestToken(retryCount, delayMilliseconds);
        }

        public async Task<CreateRequestTokenResult> ValidateRequestTokenWithLogin(string username, string password, string requestToken, int retryCount = 0, int delayMilliseconds = 1000)
        {
            return await _networkClient.ValidateRequestTokenWithLogin(username, password, requestToken, retryCount, delayMilliseconds);
        }

        public async Task<CreateSessionIdResult> CreateSessionId(string requestToken, int retryCount = 0, int delayMilliseconds = 1000)
        {
            return await _networkClient.CreateSessionId(requestToken, retryCount , delayMilliseconds);
        }

        #endregion
    }
}

using Ch9.Data.Contracts;
using System.Net;
using System.Threading.Tasks;

namespace Ch9.Services.ApiCommunicationService
{
    public class TmdbApiService
    {
        private readonly ITmdbCachedSearchClient _cachedSearchClient;

        public TmdbApiService(ITmdbCachedSearchClient cachedSearchClient)
        {
            _cachedSearchClient = cachedSearchClient;
        }

        public async Task<HttpStatusCode> TryAddMovie(int listId, int mediaId, int retryCount = 0, int delayMilliseconds = 1000)
        {
            var result = await _cachedSearchClient.AddMovie(listId, mediaId, retryCount, delayMilliseconds);
            return result.HttpStatusCode;
        }

        public async Task<HttpStatusCode> TryCreateList(string name, string description, string language = "en", int retryCount = 0, int delayMilliseconds = 1000)
        {
            var result = await _cachedSearchClient.CreateList(name, description, language, retryCount, delayMilliseconds);
            return result.HttpStatusCode;
        }
        public async Task<HttpStatusCode> TryDeleteList(int listId, int retryCount = 0, int delayMilliseconds = 1000)
        {
            var result = await _cachedSearchClient.DeleteList(listId, retryCount, delayMilliseconds);
            return result.HttpStatusCode;
        }
        public async Task<HttpStatusCode> TryDeleteMovieRating(int mediaId, string guestSessionId = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            var result = await _cachedSearchClient.DeleteMovieRating(mediaId, guestSessionId, retryCount, delayMilliseconds);
            return result.HttpStatusCode;
        }
        public async Task<HttpStatusCode> TryFetchGenreIdsWithNames(string language = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            var result = await _cachedSearchClient.FetchGenreIdsWithNames(language, retryCount, delayMilliseconds);
            return result.HttpStatusCode;
        }
        public async Task<HttpStatusCode> TryGetMovieDetails(int id, string language = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            var result = await _cachedSearchClient.GetMovieDetails(id, language, retryCount, delayMilliseconds);
            return result.HttpStatusCode;
        }
        public async Task<HttpStatusCode> TryGetMovieDetailsWithAccountStates(int id, string language = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            var result = await _cachedSearchClient.GetMovieDetailsWithAccountStates(id, language, retryCount, delayMilliseconds);
            return result.HttpStatusCode;
        }
        public async Task<HttpStatusCode> TryGetFavoriteMovies(int? accountId = null, string language = null, string sortBy = null, int? page = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            var result = await _cachedSearchClient.GetFavoriteMovies(accountId, language, sortBy, page, retryCount, delayMilliseconds);
            return result.HttpStatusCode;
        }
        public async Task<HttpStatusCode> TryGetListDetails(int listId, string language = null, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true)
        {
            var result = await _cachedSearchClient.GetListDetails(listId, language, retryCount, delayMilliseconds, fromCache);
            return result.HttpStatusCode;
        }
        public async Task<HttpStatusCode> TryGetLists(int? accountId = null, string language = null, int? page = null, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true)
        {
            var result = await _cachedSearchClient.GetLists(accountId, language, page, retryCount, delayMilliseconds, fromCache);
            return result.HttpStatusCode;
        }
        public async Task<HttpStatusCode> TryGetMovieRecommendations(int id, string language = null, int? page = null, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true)
        {
            var result = await _cachedSearchClient.GetMovieRecommendations(id, language, page, retryCount, delayMilliseconds, fromCache);
            return result.HttpStatusCode;
        }
        public async Task<HttpStatusCode> TryGetMovieReviews(int id, string language = null, int? page = null, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true)
        {
            var result = await _cachedSearchClient.GetMovieReviews(id, language, page, retryCount, delayMilliseconds, fromCache);
            return result.HttpStatusCode;
        }
        public async Task<HttpStatusCode> TryGetMovieWatchlist(int? accountId = null, string language = null, string sortBy = null, int? page = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            var result = await _cachedSearchClient.GetMovieWatchlist(accountId, language, sortBy, page, retryCount, delayMilliseconds);
            return result.HttpStatusCode;
        }
        public async Task<HttpStatusCode> TryGetSimilarMovies(int id, string language = null, int? page = null, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true)
        {
            var result = await _cachedSearchClient.GetSimilarMovies(id, language, page, retryCount, delayMilliseconds, fromCache);
            return result.HttpStatusCode;
        }
        public async Task<HttpStatusCode> TryGetTmdbConfiguration(int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true)
        {
            var result = await _cachedSearchClient.GetTmdbConfiguration(retryCount, delayMilliseconds, fromCache);
            return result.HttpStatusCode;
        }
        public async Task<HttpStatusCode> TryGetTrendingMovies(bool week = true, string language = null, bool? includeAdult = null, int? page = null, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true)
        {
            var result = await _cachedSearchClient.GetTrendingMovies(week, language, includeAdult, page, retryCount, delayMilliseconds, fromCache);
            return result.HttpStatusCode;
        }
        public async Task<HttpStatusCode> TryRateMovie(decimal rating, int mediaId, string guestSessionId = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            var result = await _cachedSearchClient.RateMovie(rating, mediaId, guestSessionId, retryCount, delayMilliseconds);
            return result.HttpStatusCode;
        }
        public async Task<HttpStatusCode> TryRemoveMovie(int listId, int mediaId, int retryCount = 0, int delayMilliseconds = 1000)
        {
            var result = await _cachedSearchClient.RemoveMovie(listId, mediaId, retryCount, delayMilliseconds);
            return result.HttpStatusCode;
        }
        public async Task<HttpStatusCode> TrySearchByMovie(string searchString, string language = null, bool? includeAdult = null, int? page = null, int? year = null, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true)
        {
            var result = await _cachedSearchClient.SearchByMovie(searchString, language, includeAdult, page, year, retryCount, delayMilliseconds, fromCache);
            return result.HttpStatusCode;
        }
        public async Task<HttpStatusCode> TryUpdateFavoriteList(string mediaType, bool add, int mediaId, int? accountId = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            var result = await _cachedSearchClient.UpdateFavoriteList(mediaType, add, mediaId, accountId, retryCount, delayMilliseconds);
            return result.HttpStatusCode;
        }
        public async Task<HttpStatusCode> TryGetMovieImages(int id, string language = null, string otherLanguage = null, bool? includeLanguageless = true, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true)
        {
            var result = await _cachedSearchClient.GetMovieImages(id, language, otherLanguage, includeLanguageless, retryCount, delayMilliseconds, fromCache);
            return result.HttpStatusCode;
        }
        public async Task<HttpStatusCode> TryUpdateWatchlist(string mediaType, bool add, int mediaId, int? accountId = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            var result = await _cachedSearchClient.UpdateWatchlist(mediaType, add, mediaId, accountId, retryCount, delayMilliseconds);
            return result.HttpStatusCode;
        }
        public async Task<HttpStatusCode> TryGetMovieVideos(int id, string language = null, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true)
        {
            var result = await _cachedSearchClient.GetMovieVideos(id, language, retryCount, delayMilliseconds, fromCache);
            return result.HttpStatusCode;
        }
        public async Task<HttpStatusCode> TryGetMovieCredits(int id, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true)
        {
            var result = await _cachedSearchClient.GetMovieCredits(id, retryCount, delayMilliseconds, fromCache);
            return result.HttpStatusCode;
        }
        public async Task<HttpStatusCode> TryGetPersonsMovieCredits(int personId, string language = null, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true)
        {
            var result = await _cachedSearchClient.GetPersonsMovieCredits(personId, language, retryCount, delayMilliseconds, fromCache);
            return result.HttpStatusCode;
        }
        public async Task<HttpStatusCode> TryGetPersonsDetails(int personId, string language = null, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true)
        {
            var result = await _cachedSearchClient.GetPersonsDetails(personId, language, retryCount, delayMilliseconds, fromCache);
            return result.HttpStatusCode;
        }
        public async Task<HttpStatusCode> TryGetAccountMovieStates(int mediaId, string guestSessionId = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            var result = await _cachedSearchClient.GetAccountMovieStates(mediaId, guestSessionId, retryCount, delayMilliseconds);
            return result.HttpStatusCode;
        }
        public async Task<HttpStatusCode> TryGetItemStatusOnTargetList(int listId, int movieId, int retryCount = 0, int delayMilliseconds = 1000)
        {
            var result = await _cachedSearchClient.GetItemStatusOnTargetList(listId, movieId, retryCount, delayMilliseconds);
            return result.HttpStatusCode;
        }
        public async Task<HttpStatusCode> TryGetPersonImages(int id, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true)
        {
            var result = await _cachedSearchClient.GetPersonImages(id, retryCount, delayMilliseconds, fromCache);
            return result.HttpStatusCode;
        }
    }
}

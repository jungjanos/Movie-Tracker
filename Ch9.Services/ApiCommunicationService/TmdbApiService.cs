using Ch9.Data.Contracts;
using Ch9.Infrastructure.Extensions;
using Ch9.Services.Contracts;
using Ch9.Ui.Contracts.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Net;
using System.Threading.Tasks;

namespace Ch9.Services.ApiCommunicationService
{
    public class TmdbApiService : ITmdbApiService
    {
        private readonly ITmdbCachedSearchClient _cachedSearchClient;

        public string SessionId
        {
            get => _cachedSearchClient.SessionId;
            set => _cachedSearchClient.SessionId = value;
        }

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
        public async Task<TryDeleteMovieRatingResponse> TryDeleteMovieRating(int mediaId, string guestSessionId = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            var response = await _cachedSearchClient.DeleteMovieRating(mediaId, guestSessionId, retryCount, delayMilliseconds);

            return new TryDeleteMovieRatingResponse(response.HttpStatusCode);
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
        /// <summary>
        /// tries to populate the existing movie object with details
        /// </summary>
        /// <param name="movieToPopulate">Movie object to populate</param>
        /// <returns>ApiCommunicationServiceResponseBase class extended with AccountMovieStates property</returns>
        public async Task<TryGetMovieDetailsWithAccountStatesResponse> TryGetMovieDetailsWithAccountStates(MovieDetailModel movieToPopulate, int id, string language = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            var response = await _cachedSearchClient.GetMovieDetailsWithAccountStates(id, language, retryCount, delayMilliseconds);
            AccountMovieStates states = null;

            if (response.HttpStatusCode.IsSuccessCode())
            {
                if (movieToPopulate != null)
                    JsonConvert.PopulateObject(response.Json, movieToPopulate);

                if (response.HttpStatusCode == HttpStatusCode.OK)
                {
                    var jsonSettings = new JsonSerializerSettings()
                    { Error = delegate (object sender, ErrorEventArgs args) { args.ErrorContext.Handled = true; } };
                    var tmdbResponse = JObject.Parse(response.Json);
                    var result = tmdbResponse["account_states"];

                    if (result != null)
                        states = result.ToObject<AccountMovieStates>(JsonSerializer.Create(jsonSettings));
                }
            }
            return new TryGetMovieDetailsWithAccountStatesResponse(response.HttpStatusCode, states);
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
        public async Task<TryGetMovieRecommendationsResponse> TryGetMovieRecommendations(int id, string language = null, int? page = null, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true)
        {
            var response = await _cachedSearchClient.GetMovieRecommendations(id, language, page, retryCount, delayMilliseconds, fromCache);
            SearchResult recommendations = null;

            if (response.HttpStatusCode.IsSuccessCode())
                recommendations = JsonConvert.DeserializeObject<SearchResult>(response.Json);

            return new TryGetMovieRecommendationsResponse(response.HttpStatusCode, recommendations);
        }
        public async Task<TryGetMovieReviewsResponse> TryGetMovieReviews(int id, string language = null, int? page = null, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true)
        {
            var response = await _cachedSearchClient.GetMovieReviews(id, language, page, retryCount, delayMilliseconds, fromCache);
            ReviewsModel reviews = null;

            if (response.HttpStatusCode.IsSuccessCode())
                reviews = JsonConvert.DeserializeObject<ReviewsModel>(response.Json);

            return new TryGetMovieReviewsResponse(response.HttpStatusCode, reviews);
        }
        public async Task<HttpStatusCode> TryGetMovieWatchlist(int? accountId = null, string language = null, string sortBy = null, int? page = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            var result = await _cachedSearchClient.GetMovieWatchlist(accountId, language, sortBy, page, retryCount, delayMilliseconds);
            return result.HttpStatusCode;
        }
        public async Task<TryGetSimilarMoviesResponse> TryGetSimilarMovies(int id, string language = null, int? page = null, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true)
        {
            var response = await _cachedSearchClient.GetSimilarMovies(id, language, page, retryCount, delayMilliseconds, fromCache);
            SearchResult similars = null;

            if (response.HttpStatusCode.IsSuccessCode())
                similars = JsonConvert.DeserializeObject<SearchResult>(response.Json);

            return new TryGetSimilarMoviesResponse(response.HttpStatusCode, similars);
        }
        //public async Task<HttpStatusCode> TryGetTmdbConfiguration(int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true)
        //{
        //    throw new System.Exception();
        //    var response = await _cachedSearchClient.GetTmdbConfiguration(retryCount, delayMilliseconds, fromCache);
        //    TmdbConfigurationModel 

        //    return result.HttpStatusCode;
        //}
        public async Task<TryGetTrendingMoviesResponse> TryGetTrendingMovies(bool week = true, string language = null, bool? includeAdult = null, int? page = null, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true)
        {
            var response = await _cachedSearchClient.GetTrendingMovies(week, language, includeAdult, page, retryCount, delayMilliseconds, fromCache);
            SearchResult movies = null;

            if (response.HttpStatusCode.IsSuccessCode())
                movies = JsonConvert.DeserializeObject<SearchResult>(response.Json);

            return new TryGetTrendingMoviesResponse(response.HttpStatusCode, movies);
        }
        public async Task<TryRateMovieResponse> TryRateMovie(decimal rating, int mediaId, string guestSessionId = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            var response = await _cachedSearchClient.RateMovie(rating, mediaId, guestSessionId, retryCount, delayMilliseconds);

            return new TryRateMovieResponse(response.HttpStatusCode);
        }
        public async Task<HttpStatusCode> TryRemoveMovie(int listId, int mediaId, int retryCount = 0, int delayMilliseconds = 1000)
        {
            var result = await _cachedSearchClient.RemoveMovie(listId, mediaId, retryCount, delayMilliseconds);
            return result.HttpStatusCode;
        }
        public async Task<TrySearchByMovieResponse> TrySearchByMovie(string searchString, string language = null, bool? includeAdult = null, int? page = null, int? year = null, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true)
        {
            var response = await _cachedSearchClient.SearchByMovie(searchString, language, includeAdult, page, year, retryCount, delayMilliseconds, fromCache);
            SearchResult searchResult = null;

            if (response.HttpStatusCode.IsSuccessCode())
                searchResult = JsonConvert.DeserializeObject<SearchResult>(response.Json);

            return new TrySearchByMovieResponse(response.HttpStatusCode, searchResult);
        }
        public async Task<HttpStatusCode> TryUpdateFavoriteList(string mediaType, bool add, int mediaId, int? accountId = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            var result = await _cachedSearchClient.UpdateFavoriteList(mediaType, add, mediaId, accountId, retryCount, delayMilliseconds);
            return result.HttpStatusCode;
        }
        public async Task<TryGetMovieImagesResponse> TryGetMovieImages(int id, string language = null, string otherLanguage = null, bool? includeLanguageless = true, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true)
        {
            var response = await _cachedSearchClient.GetMovieImages(id, language, otherLanguage, includeLanguageless, retryCount, delayMilliseconds, fromCache);
            ImageDetailCollection imageCollection = null;

            if (response.HttpStatusCode.IsSuccessCode())
                imageCollection = JsonConvert.DeserializeObject<ImageDetailCollection>(response.Json);

            return new TryGetMovieImagesResponse(response.HttpStatusCode, imageCollection);
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
        public async Task<TryGetMovieCreditsResponse> TryGetMovieCredits(int id, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true)
        {
            var response = await _cachedSearchClient.GetMovieCredits(id, retryCount, delayMilliseconds, fromCache);
            MovieCreditsModel movieCredits = null;

            if (response.HttpStatusCode.IsSuccessCode())
                movieCredits = JsonConvert.DeserializeObject<MovieCreditsModel>(response.Json);

            return new TryGetMovieCreditsResponse(response.HttpStatusCode, movieCredits);
        }
        public async Task<TryGetPersonsMovieCreditsResponse> TryGetPersonsMovieCredits(int personId, string language = null, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true)
        {
            var response = await _cachedSearchClient.GetPersonsMovieCredits(personId, language, retryCount, delayMilliseconds, fromCache);

            PersonsMovieCreditsModel personsMovieCredits = null;

            if (response.HttpStatusCode.IsSuccessCode())
                personsMovieCredits = JsonConvert.DeserializeObject<PersonsMovieCreditsModel>(response.Json);

            return new TryGetPersonsMovieCreditsResponse(response.HttpStatusCode, personsMovieCredits);
        }
        public async Task<TryGetPersonsDetailsResponse> TryGetPersonsDetails(int personId, string language = null, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true)
        {
            var response = await _cachedSearchClient.GetPersonsDetails(personId, language, retryCount, delayMilliseconds, fromCache);
            PersonsDetailsModel personsDetails = null;

            if (response.HttpStatusCode.IsSuccessCode())
                personsDetails = JsonConvert.DeserializeObject<PersonsDetailsModel>(response.Json);

            return new TryGetPersonsDetailsResponse(response.HttpStatusCode, personsDetails);
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
        public async Task<TryGetPersonImagesResponse> TryGetPersonImages(int id, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true)
        {
            var response = await _cachedSearchClient.GetPersonImages(id, retryCount, delayMilliseconds, fromCache);
            ImageModel[] images = null;

            if (response.HttpStatusCode.IsSuccessCode())
                images = (JsonConvert.DeserializeObject<ImageDetailCollection>(response.Json)).Profiles;

            return new TryGetPersonImagesResponse(response.HttpStatusCode, images);
        }

        public async Task<TryDeleteSessionResponse> TryDeleteSession(string sessionId, int retryCount = 0, int delayMilliseconds = 1000)
        {
            var response = await _cachedSearchClient.DeleteSession(sessionId, retryCount, delayMilliseconds);

            return new TryDeleteSessionResponse(response.HttpStatusCode);
        }

        public async Task<TryCreateRequestTokenResponse> TryCreateRequestToken(int retryCount = 0, int delayMilliseconds = 1000)
        {
            var response = await _cachedSearchClient.CreateRequestToken(retryCount, delayMilliseconds);
            RequestToken requestToken = null;

            if (response.HttpStatusCode.IsSuccessCode())
                requestToken = JsonConvert.DeserializeObject<RequestToken>(response.Json);

            return new TryCreateRequestTokenResponse(response.HttpStatusCode, requestToken);
        }

        public async Task<TryValidateRequestTokenWithLoginResponse> TryValidateRequestTokenWithLogin(string username, string password, string requestToken, int retryCount = 0, int delayMilliseconds = 1000)
        {
            var response = await _cachedSearchClient.ValidateRequestTokenWithLogin(username, password, requestToken, retryCount, delayMilliseconds);
            RequestToken token = null;

            if (response.HttpStatusCode.IsSuccessCode())
                token = JsonConvert.DeserializeObject<RequestToken>(response.Json);

            return new TryValidateRequestTokenWithLoginResponse(response.HttpStatusCode, token);
        }

        public async Task<TryCreateSessionIdResponse> TryCreateSessionId(string requestToken, int retryCount = 0, int delayMilliseconds = 1000)
        {
            var response = await _cachedSearchClient.CreateSessionId(requestToken, retryCount, delayMilliseconds);
            SessionIdResponseModel sessionIdResponse = null;

            if (response.HttpStatusCode.IsSuccessCode())
                sessionIdResponse = JsonConvert.DeserializeObject<SessionIdResponseModel>(response.Json);

            return new TryCreateSessionIdResponse(response.HttpStatusCode, sessionIdResponse);
        }
    }
}

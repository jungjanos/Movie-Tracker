using Ch9.Services;
using Ch9.Ui.Contracts.Models;
using Ch9.Utils;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static Ch9.ApiClient.WebApiPathConstants;

namespace Ch9.ApiClient
{
    // TODO : SessionId should be injected into the public methods via parameter instead of grabbing it from ISettings
    // This will help with caching (SessionId needs to be used as part of the caching key to differentiate between users)
    public class TmdbNetworkClient : ITmdbNetworkClient
    {
        private readonly ISettings _settings;
        private HttpClient HttpClient { get; }

        public TmdbNetworkClient(ISettings settings, HttpClient httpClient)
        {
            _settings = settings;
            HttpClient = httpClient ?? new HttpClient();
        }

        public async Task<TmdbConfigurationModelResult> GetTmdbConfiguration(int retryCount = 0, int delayMilliseconds = 1000)
        {
            string baseUrl = BASE_Address + BASE_Path + CONFIG_Path;

            var query = new Dictionary<string, string>();
            query.Add(API_KEY_Key, _settings.ApiKey);

            string requestUri = QueryHelpers.AddQueryString(baseUrl, query);

            var result = await GetResponse<TmdbConfigurationModelResult>(retryCount, delayMilliseconds, requestUri);

            return result;
        }

        // Fetches Genre information from WebAPI
        // indicates success or failure as Http code,
        // retries as required
        //ToDo: magic string (language should be replaced by enum type) 
        public async Task<GenreNameFetchResult> FetchGenreIdsWithNames(string language = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            string baseUrl = BASE_Address + BASE_Path + GENRE_LIST_Path;

            var query = new Dictionary<string, string>();
            query.Add(API_KEY_Key, _settings.ApiKey);

            if (!string.IsNullOrEmpty(language))
                query.Add(LANGUAGE_Key, language);

            string requestUri = QueryHelpers.AddQueryString(baseUrl, query);

            GenreNameFetchResult result = await GetResponse<GenreNameFetchResult>(retryCount, delayMilliseconds, requestUri);

            return result;
        }

        // Searches for moves according to settings 
        // Swallows exceptions retries as needed    
        public async Task<SearchByMovieResult> SearchByMovie(string searchString, string language = null, bool? includeAdult = null, int? page = null, int? year = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            string baseUrl = BASE_Address + BASE_Path + SEARCH_MOVIE_Path;

            var query = new Dictionary<string, string>();
            query.Add(API_KEY_Key, _settings.ApiKey);
            query.Add(SEARCH_QUERY_Key, searchString);

            if (!string.IsNullOrEmpty(language))
                query.Add(LANGUAGE_Key, language);

            if (includeAdult != null)
            {
                if (includeAdult.Value)
                    query.Add(INCLUDE_Adult_Key, "true");
                else
                    query.Add(INCLUDE_Adult_Key, "false");
            }

            if (page > 0)
                query.Add(PAGE_Key, page.ToString());

            if (year > 0)
                query.Add(YEAR_Key, year.ToString());

            string requestUri = QueryHelpers.AddQueryString(baseUrl, query);

            SearchByMovieResult result = await GetResponse<SearchByMovieResult>(retryCount, delayMilliseconds, requestUri);

            return result;
        }
        //TODO : Make sort option to enum 
        public async Task<GetMovieWatchlistResult> GetMovieWatchlist(int? accountId = null, string language = null, string sortBy = null, int? page = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            //TODO : Recheck this!
            // no mistake here: missing "account_id" parameter add the string literal "{account_id}" as paths segment                        
            string baseUrl = BASE_Address + BASE_Path + ACCOUNT_DETAILS_Path + "/" + (accountId.HasValue ? accountId.Value.ToString() : "{account_id}") + WATCHLIST_Path + MOVIES_Path;

            var query = new Dictionary<string, string>();
            query.Add(API_KEY_Key, _settings.ApiKey);

            if (!string.IsNullOrEmpty(language))
                query.Add(LANGUAGE_Key, language);

            if (page > 0)
                query.Add(PAGE_Key, page.Value.ToString());

            if (!string.IsNullOrEmpty(sortBy))
                query.Add(SORTBY_Key, sortBy);

            query.Add(SESSION_ID_Key, _settings.SessionId);

            string requestUri = QueryHelpers.AddQueryString(baseUrl, query);

            GetMovieWatchlistResult result = await GetResponse<GetMovieWatchlistResult>(retryCount, delayMilliseconds, requestUri);

            return result;
        }

        //TODO : Make sort option to enum 
        public async Task<GetFavoriteMoviesResult> GetFavoriteMovies(int? accountId = null, string language = null, string sortBy = null, int? page = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            // no mistake here: missing "account_id" parameter add the string literal "{account_id}" as paths segment                        
            string baseUrl = BASE_Address + BASE_Path + ACCOUNT_DETAILS_Path + "/" + (accountId.HasValue ? accountId.Value.ToString() : "{account_id}") + FAVORITE_Path + MOVIES_Path;

            var query = new Dictionary<string, string>();
            query.Add(API_KEY_Key, _settings.ApiKey);

            if (!string.IsNullOrEmpty(language))
                query.Add(LANGUAGE_Key, language);

            if (page > 0)
                query.Add(PAGE_Key, page.Value.ToString());

            if (!string.IsNullOrEmpty(sortBy))
                query.Add(SORTBY_Key, sortBy);

            query.Add(SESSION_ID_Key, _settings.SessionId);

            string requestUri = QueryHelpers.AddQueryString(baseUrl, query);

            GetFavoriteMoviesResult result = await GetResponse<GetFavoriteMoviesResult>(retryCount, delayMilliseconds, requestUri);

            return result;
        }

        public async Task<GetMovieDetailsResult> GetMovieDetails(int id, string language = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            string baseUrl = BASE_Address + BASE_Path + MOVIE_DETAILS_Path + "/" + id;

            var query = new Dictionary<string, string>();
            query.Add(API_KEY_Key, _settings.ApiKey);

            if (!string.IsNullOrEmpty(language))
                query.Add(LANGUAGE_Key, language);

            string requestUri = QueryHelpers.AddQueryString(baseUrl, query);

            GetMovieDetailsResult result = await GetResponse<GetMovieDetailsResult>(retryCount, delayMilliseconds, requestUri);

            return result;
        }

        public async Task<GetMovieDetailsWithAccountStatesResult> GetMovieDetailsWithAccountStates(int id, string language = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            string baseUrl = BASE_Address + BASE_Path + MOVIE_DETAILS_Path + "/" + id;

            var query = new Dictionary<string, string>();
            query.Add(API_KEY_Key, _settings.ApiKey);
            if (!string.IsNullOrEmpty(language))
                query.Add(LANGUAGE_Key, language);

            if (!string.IsNullOrEmpty(_settings.SessionId))
            {
                query.Add(SESSION_ID_Key, _settings.SessionId);
                query.Add(APPEND_RESPONSE_Key, ACCOUNT_STATES_Key);
            }

            string requestUri = QueryHelpers.AddQueryString(baseUrl, query);

            GetMovieDetailsWithAccountStatesResult result = await GetResponse<GetMovieDetailsWithAccountStatesResult>(retryCount, delayMilliseconds, requestUri);

            return result;
        }

        // Fetches the list of trending moves according to query
        // swallows exceptions, retries as configured
        public async Task<TrendingMoviesResult> GetTrendingMovies(bool week = true, string language = null, bool? includeAdult = null, int? page = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            string baseUrl = BASE_Address + BASE_Path + TRENDING_Path + TRENDING_MOVIE_Selector + (week ? WEEK_Path : DAY_Path);

            var query = new Dictionary<string, string>();
            query.Add(API_KEY_Key, _settings.ApiKey);

            if (!string.IsNullOrEmpty(language))
                query.Add(LANGUAGE_Key, language);

            if (includeAdult != null)
            {
                if (includeAdult.Value)
                    query.Add(INCLUDE_Adult_Key, "true");
                else
                    query.Add(INCLUDE_Adult_Key, "false");
            }

            if (page > 0)
                query.Add(PAGE_Key, page.Value.ToString());

            string requestUri = QueryHelpers.AddQueryString(baseUrl, query);

            TrendingMoviesResult result = await GetResponse<TrendingMoviesResult>(retryCount, delayMilliseconds, requestUri);

            return result;
        }

        public async Task<GetMovieImagesResult> GetMovieImages(int id, string language = null, string otherLanguage = null, bool? includeLanguageless = true, int retryCount = 0, int delayMilliseconds = 1000)
        {
            string baseUrl = BASE_Address + BASE_Path + MOVIE_DETAILS_Path + "/" + id + IMAGE_DETAIL_Path;
            var query = new Dictionary<string, string>();
            query.Add(API_KEY_Key, _settings.ApiKey);

            if (!string.IsNullOrEmpty(language))
                query.Add(LANGUAGE_Key, language);

            List<string> otherLanguages = new List<string>();
            if (!string.IsNullOrEmpty(otherLanguage))
                otherLanguages.Add(otherLanguage);

            // for the TMDB WebAPI 'null' means include languageless pictures 
            if (includeLanguageless == true)
                otherLanguages.Add("null");

            var otherLanguagesValue = string.Join(",", otherLanguages);

            if (!string.IsNullOrWhiteSpace(otherLanguagesValue))
                query.Add(IMAGE_ADDITIONAL_LANGUAGES, otherLanguagesValue);

            string requestUri = QueryHelpers.AddQueryString(baseUrl, query);
            GetMovieImagesResult result = await GetResponse<GetMovieImagesResult>(retryCount, delayMilliseconds, requestUri);

            return result;
        }

        public async Task<GetPersonImagesResult> GetPersonImages(int id, int retryCount = 0, int delayMilliseconds = 1000)
        {
            string baseUrl = BASE_Address + BASE_Path + PERSON_Path + "/" + id + IMAGE_DETAIL_Path;

            var query = new Dictionary<string, string>();
            query.Add(API_KEY_Key, _settings.ApiKey);

            string requestUri = QueryHelpers.AddQueryString(baseUrl, query);

            GetPersonImagesResult result = await GetResponse<GetPersonImagesResult>(retryCount, delayMilliseconds, requestUri);

            return result;
        }

        public async Task<GetMovieVideosResult> GetMovieVideos(int id, string language = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            string baseUrl = BASE_Address + BASE_Path + MOVIE_DETAILS_Path + "/" + id + VIDEO_DETAIL_PATH;
            var query = new Dictionary<string, string>();
            query.Add(API_KEY_Key, _settings.ApiKey);

            if (!string.IsNullOrEmpty(language))
                query.Add(LANGUAGE_Key, language);

            string requestUri = QueryHelpers.AddQueryString(baseUrl, query);
            GetMovieVideosResult result = await GetResponse<GetMovieVideosResult>(retryCount, delayMilliseconds, requestUri);

            return result;
        }

        public async Task<GetMovieRecommendationsResult> GetMovieRecommendations(int id, string language = null, int? page = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            string baseUrl = BASE_Address + BASE_Path + MOVIE_DETAILS_Path + "/" + id + RECOMMENDATIONS_Path;

            var query = new Dictionary<string, string>();
            query.Add(API_KEY_Key, _settings.ApiKey);

            if (!string.IsNullOrEmpty(language))
                query.Add(LANGUAGE_Key, language);

            if (page > 0)
                query.Add(PAGE_Key, page.ToString());

            string requestUri = QueryHelpers.AddQueryString(baseUrl, query);

            GetMovieRecommendationsResult result = await GetResponse<GetMovieRecommendationsResult>(retryCount, delayMilliseconds, requestUri);

            return result;
        }

        public async Task<GetSimilarMoviesResult> GetSimilarMovies(int id, string language = null, int? page = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            string baseUrl = BASE_Address + BASE_Path + MOVIE_DETAILS_Path + "/" + id + SIMILARS_Path;

            var query = new Dictionary<string, string>();
            query.Add(API_KEY_Key, _settings.ApiKey);

            if (!string.IsNullOrEmpty(language))
                query.Add(LANGUAGE_Key, language);

            if (page > 0)
                query.Add(PAGE_Key, page.ToString());

            string requestUri = QueryHelpers.AddQueryString(baseUrl, query);

            GetSimilarMoviesResult result = await GetResponse<GetSimilarMoviesResult>(retryCount, delayMilliseconds, requestUri);

            return result;
        }

        public async Task<GetMovieReviewsResult> GetMovieReviews(int id, string language = null, int? page = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            string baseUrl = BASE_Address + BASE_Path + MOVIE_DETAILS_Path + "/" + id + REVIEWS_Path;

            var query = new Dictionary<string, string>();
            query.Add(API_KEY_Key, _settings.ApiKey);

            if (!string.IsNullOrEmpty(language))
                query.Add(LANGUAGE_Key, language);

            if (page > 0)
                query.Add(PAGE_Key, page.Value.ToString());

            string requestUri = QueryHelpers.AddQueryString(baseUrl, query);

            GetMovieReviewsResult result = await GetResponse<GetMovieReviewsResult>(retryCount, delayMilliseconds, requestUri);

            return result;
        }

        public async Task<GetMovieCreditsResult> GetMovieCredits(int id, int retryCount = 0, int delayMilliseconds = 1000)
        {
            string baseUrl = BASE_Address + BASE_Path + MOVIE_DETAILS_Path + "/" + id + CREDITS_Path;

            var query = new Dictionary<string, string>();
            query.Add(API_KEY_Key, _settings.ApiKey);

            string requestUri = QueryHelpers.AddQueryString(baseUrl, query);

            GetMovieCreditsResult result = await GetResponse<GetMovieCreditsResult>(retryCount, delayMilliseconds, requestUri);

            return result;
        }

        public async Task<GetPersonsMovieCreditsResult> GetPersonsMovieCredits(int personId, string language = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            string baseUrl = BASE_Address + BASE_Path + PERSON_Path + "/" + personId + MOVIE_CREDITS_Path;

            var query = new Dictionary<string, string>();
            query.Add(API_KEY_Key, _settings.ApiKey);

            if (!string.IsNullOrEmpty(language))
                query.Add(LANGUAGE_Key, language);

            string requestUri = QueryHelpers.AddQueryString(baseUrl, query);

            GetPersonsMovieCreditsResult result = await GetResponse<GetPersonsMovieCreditsResult>(retryCount, delayMilliseconds, requestUri);

            return result;
        }

        public async Task<GetPersonsDetailsResult> GetPersonsDetails(int personId, string language = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            string baseUrl = BASE_Address + BASE_Path + PERSON_Path + "/" + personId;

            var query = new Dictionary<string, string>();
            query.Add(API_KEY_Key, _settings.ApiKey);

            if (!string.IsNullOrEmpty(language))
                query.Add(LANGUAGE_Key, language);

            string requestUri = QueryHelpers.AddQueryString(baseUrl, query);

            GetPersonsDetailsResult result = await GetResponse<GetPersonsDetailsResult>(retryCount, delayMilliseconds, requestUri);

            return result;
        }

        public async Task<CreateRequestTokenResult> CreateRequestToken(int retryCount = 0, int delayMilliseconds = 1000)
        {
            string baseUrl = BASE_Address + BASE_Path + REQUEST_TOKEN_Path;

            var query = new Dictionary<string, string>();
            query.Add(API_KEY_Key, _settings.ApiKey);

            string requestUri = QueryHelpers.AddQueryString(baseUrl, query);

            CreateRequestTokenResult result = await GetResponse<CreateRequestTokenResult>(retryCount, delayMilliseconds, requestUri);

            return result;
        }

        public async Task<CreateRequestTokenResult> ValidateRequestTokenWithLogin(string username, string password, string requestToken, int retryCount = 0, int delayMilliseconds = 1000)
        {
            string baseUrl = BASE_Address + BASE_Path + VALIDATE_REQUEST_TOKEN_W_LOGIN_Path;

            var query = new Dictionary<string, string>();
            query.Add(API_KEY_Key, _settings.ApiKey);

            string requestUri = QueryHelpers.AddQueryString(baseUrl, query);
            var jsonObj = new { username = username, password = password, request_token = requestToken };
            string json = JsonConvert.SerializeObject(jsonObj);
            var content = new StringContent(json, encoding: Encoding.UTF8, mediaType: "application/json");

            HttpResponseMessage response = null;
            int counter = retryCount;

            try
            {
                response = await HttpClient.PostAsync(requestUri, content);
            }
            catch { }
            while (response?.IsSuccessStatusCode != true && counter > 0)
            {
                await Task.Delay(delayMilliseconds);
                try
                {
                    --counter;
                    response = await HttpClient.PostAsync(requestUri, content);
                }
                catch { }
            }
            CreateRequestTokenResult result = new CreateRequestTokenResult { HttpStatusCode = response?.StatusCode ?? HttpStatusCode.ServiceUnavailable };

            await ReadResponseAsStringIntoResultWhenSafe(result, response);

            return result;
        }

        public async Task<CreateSessionIdResult> CreateSessionId(string requestToken, int retryCount = 0, int delayMilliseconds = 1000)
        {
            string baseUrl = BASE_Address + BASE_Path + CREATE_SESSION_ID_Path;

            var query = new Dictionary<string, string>();
            query.Add(API_KEY_Key, _settings.ApiKey);

            string requestUri = QueryHelpers.AddQueryString(baseUrl, query);

            var jsonObj = new { request_token = requestToken };
            string json = JsonConvert.SerializeObject(jsonObj);
            var content = new StringContent(json, encoding: Encoding.UTF8, mediaType: "application/json");

            HttpResponseMessage response = null;
            int counter = retryCount;

            try
            {
                response = await HttpClient.PostAsync(requestUri, content);
            }
            catch { }
            while (response?.IsSuccessStatusCode != true && counter > 0)
            {
                await Task.Delay(delayMilliseconds);
                try
                {
                    --counter;
                    response = await HttpClient.PostAsync(requestUri, content);
                }
                catch { }
            }
            CreateSessionIdResult result = new CreateSessionIdResult { HttpStatusCode = response?.StatusCode ?? HttpStatusCode.ServiceUnavailable };

            await ReadResponseAsStringIntoResultWhenSafe(result, response);

            return result;
        }

        public async Task<DeleteSessionResult> DeleteSession(string sessionId, int retryCount = 0, int delayMilliseconds = 1000)
        {
            string baseUrl = BASE_Address + BASE_Path + DELETE_SESSION_Path;

            var query = new Dictionary<string, string>();
            query.Add(API_KEY_Key, _settings.ApiKey);

            string requestUri = QueryHelpers.AddQueryString(baseUrl, query);

            var jsonObj = new { session_id = sessionId };
            string json = JsonConvert.SerializeObject(jsonObj);
            var content = new StringContent(json, encoding: Encoding.UTF8, mediaType: "application/json");

            HttpRequestMessage request = new HttpRequestMessage
            {
                Content = content,
                Method = HttpMethod.Delete,
                RequestUri = new Uri(requestUri)
            };

            HttpResponseMessage response = null;
            int counter = retryCount;

            try
            {
                response = await HttpClient.SendAsync(request);
            }
            catch { }
            while (response?.IsSuccessStatusCode != true && counter > 0)
            {
                await Task.Delay(delayMilliseconds);
                try
                {
                    --counter;
                    response = await HttpClient.SendAsync(request);
                }
                catch { }
            }

            DeleteSessionResult result = new DeleteSessionResult
            {
                HttpStatusCode = response?.StatusCode ?? HttpStatusCode.RequestTimeout
            };

            await ReadResponseAsStringIntoResultWhenSafe(result, response);

            return result;
        }

        public async Task<GetAccountDetailsResult> GetAccountDetails(string sessionId, int retryCount = 0, int delayMilliseconds = 1000)
        {
            string baseUrl = BASE_Address + BASE_Path + ACCOUNT_DETAILS_Path;

            var query = new Dictionary<string, string>();
            query.Add(API_KEY_Key, _settings.ApiKey);
            query.Add(SESSION_ID_Key, sessionId);

            string requestUri = QueryHelpers.AddQueryString(baseUrl, query);

            GetAccountDetailsResult result = await GetResponse<GetAccountDetailsResult>(retryCount, delayMilliseconds, requestUri);

            return result;
        }

        public async Task<GetListsResult> GetLists(int? accountId = null, string language = null, int? page = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            // no mistake here: missing "account_id" parameter add the string literal "{account_id}" as paths segment            
            string baseUrl = BASE_Address + BASE_Path + ACCOUNT_DETAILS_Path + "/" + (accountId.HasValue ? accountId.Value.ToString() : "{account_id}") + LISTS_path;

            var query = new Dictionary<string, string>();
            query.Add(API_KEY_Key, _settings.ApiKey);
            if (!string.IsNullOrEmpty(language))
                query.Add(LANGUAGE_Key, language);
            if (page > 0)
                query.Add(PAGE_Key, page.Value.ToString());
            query.Add(SESSION_ID_Key, _settings.SessionId);

            string requestUri = QueryHelpers.AddQueryString(baseUrl, query);

            GetListsResult result = await GetResponse<GetListsResult>(retryCount, delayMilliseconds, requestUri);
            return result;
        }

        public async Task<GetAccountMovieStatesResult> GetAccountMovieStates(int mediaId, string guestSessionId = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            if (!string.IsNullOrEmpty(guestSessionId))
                throw new NotImplementedException($"Guest session is not supported by the method: {nameof(GetAccountMovieStates)}, parameter: {nameof(guestSessionId)}={guestSessionId}");

            string baseUrl = BASE_Address + BASE_Path + MOVIE_DETAILS_Path + "/" + mediaId + ACCOUNT_STATES_Path;

            var query = new Dictionary<string, string>();
            query.Add(API_KEY_Key, _settings.ApiKey);
            query.Add(SESSION_ID_Key, _settings.SessionId);

            string requestUri = QueryHelpers.AddQueryString(baseUrl, query);

            GetAccountMovieStatesResult result = await GetResponse<GetAccountMovieStatesResult>(retryCount, delayMilliseconds, requestUri);

            return result;
        }

        public async Task<CreateListResult> CreateList(string name, string description, string language = "en", int retryCount = 0, int delayMilliseconds = 1000)
        {
            string baseUrl = BASE_Address + BASE_Path + LIST_path;

            var query = new Dictionary<string, string>();
            query.Add(API_KEY_Key, _settings.ApiKey);
            query.Add(SESSION_ID_Key, _settings.SessionId);


            string requestUri = QueryHelpers.AddQueryString(baseUrl, query);

            var jsonObj = new
            {
                name = name,
                description = description,
                language = language
            };
            string json = JsonConvert.SerializeObject(jsonObj);
            var content = new StringContent(json, encoding: Encoding.UTF8, mediaType: "application/json");

            HttpRequestMessage request = new HttpRequestMessage
            {
                Content = content,
                Method = HttpMethod.Post,
                RequestUri = new Uri(requestUri)
            };

            HttpResponseMessage response = null;
            int counter = retryCount;

            try
            {
                response = await HttpClient.SendAsync(request);
            }
            catch { }
            while (response?.IsSuccessStatusCode != true && counter > 0)
            {
                await Task.Delay(delayMilliseconds);
                try
                {
                    --counter;
                    response = await HttpClient.SendAsync(request);
                }
                catch { }
            }

            CreateListResult result = new CreateListResult
            {
                HttpStatusCode = response?.StatusCode ?? HttpStatusCode.RequestTimeout
            };

            await ReadResponseAsStringIntoResultWhenSafe(result, response);

            return result;
        }

        // The TMDB WebAPI Has a glitch here:
        // when removing an existing list, Http.500 denotes 'success'
        public async Task<DeleteListResult> DeleteList(int listId, int retryCount = 0, int delayMilliseconds = 1000)
        {
            string baseUrl = BASE_Address + BASE_Path + LIST_path + "/" + listId;

            var query = new Dictionary<string, string>();
            query.Add(API_KEY_Key, _settings.ApiKey);
            query.Add(SESSION_ID_Key, _settings.SessionId);

            string requestUri = QueryHelpers.AddQueryString(baseUrl, query);

            HttpResponseMessage response = null;
            int counter = retryCount;

            try
            {
                response = await HttpClient.DeleteAsync(requestUri);
            }
            catch { }
            while (response?.StatusCode != HttpStatusCode.InternalServerError && counter > 0)
            {
                await Task.Delay(delayMilliseconds);
                try
                {
                    --counter;
                    response = await HttpClient.DeleteAsync(requestUri);
                }
                catch { }
            }

            DeleteListResult result = new DeleteListResult
            {
                HttpStatusCode = response?.StatusCode ?? HttpStatusCode.RequestTimeout
            };

            await ReadResponseAsStringIntoResultWhenSafe(result, response);

            return result;
        }

        public async Task<GetListDetailsResult> GetListDetails(int listId, string language = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            string baseUrl = BASE_Address + BASE_Path + LIST_path + "/" + listId;

            var query = new Dictionary<string, string>();
            query.Add(API_KEY_Key, _settings.ApiKey);
            query.Add(SESSION_ID_Key, _settings.SessionId);
            if (!string.IsNullOrWhiteSpace(language))
                query.Add(LANGUAGE_Key, language);

            string requestUri = QueryHelpers.AddQueryString(baseUrl, query);

            GetListDetailsResult result = await GetResponse<GetListDetailsResult>(retryCount, delayMilliseconds, requestUri);

            return result;
        }

        // https://developers.themoviedb.org/3/lists/check-item-status
        public async Task<GetItemStatusOnTargetListResult> GetItemStatusOnTargetList(int listId, int movieId, int retryCount = 0, int delayMilliseconds = 1000)
        {
            string baseUrl = BASE_Address + BASE_Path + LIST_path + "/" + listId + ITEM_STATUS_Path;

            var query = new Dictionary<string, string>();
            query.Add(API_KEY_Key, _settings.ApiKey);
            query.Add(MOVIE_ID_Key, movieId.ToString());

            string requestUri = QueryHelpers.AddQueryString(baseUrl, query);

            GetItemStatusOnTargetListResult result = await GetResponse<GetItemStatusOnTargetListResult>(retryCount, delayMilliseconds, requestUri);

            return result;
        }

        public async Task<AddMovieResult> AddMovie(int listId, int mediaId, int retryCount = 0, int delayMilliseconds = 1000)
        {
            string baseUrl = BASE_Address + BASE_Path + LIST_path + "/" + listId + ADD_MEDIA_Path;

            var query = new Dictionary<string, string>();
            query.Add(API_KEY_Key, _settings.ApiKey);
            query.Add(SESSION_ID_Key, _settings.SessionId);

            string requestUri = QueryHelpers.AddQueryString(baseUrl, query);

            var jsonObj = new
            {
                media_id = mediaId
            };

            string json = JsonConvert.SerializeObject(jsonObj);
            var content = new StringContent(json, encoding: Encoding.UTF8, mediaType: "application/json");

            HttpRequestMessage request = new HttpRequestMessage
            {
                Content = content,
                Method = HttpMethod.Post,
                RequestUri = new Uri(requestUri)
            };

            HttpResponseMessage response = null;
            int counter = retryCount;

            try
            {
                response = await HttpClient.SendAsync(request);
            }
            catch { }
            while (response?.IsSuccessStatusCode != true && counter > 0)
            {
                await Task.Delay(delayMilliseconds);
                try
                {
                    --counter;
                    response = await HttpClient.SendAsync(request);
                }
                catch { }
            }

            AddMovieResult result = new AddMovieResult
            {
                HttpStatusCode = response?.StatusCode ?? HttpStatusCode.RequestTimeout
            };

            await ReadResponseAsStringIntoResultWhenSafe(result, response);

            return result;
        }

        // adds or removes media from watchlist
        // media type: "movie" OR "tv"
        //TODO refactor media type to enum
        public async Task<UpdateWatchlistResult> UpdateWatchlist(string mediaType, bool add, int mediaId, int? accountId = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            // no mistake here: missing "account_id" parameter add the string literal "{account_id}" as paths segment                        
            string baseUrl = BASE_Address + BASE_Path + ACCOUNT_DETAILS_Path + "/" + (accountId.HasValue ? accountId.Value.ToString() : "{account_id}") + WATCHLIST_Path;

            var query = new Dictionary<string, string>();
            query.Add(API_KEY_Key, _settings.ApiKey);
            query.Add(SESSION_ID_Key, _settings.SessionId);

            string requestUri = QueryHelpers.AddQueryString(baseUrl, query);

            var jsonObj = new
            {
                media_type = mediaType,
                media_id = mediaId,
                watchlist = add
            };

            string json = JsonConvert.SerializeObject(jsonObj);
            var content = new StringContent(json, encoding: Encoding.UTF8, mediaType: "application/json");

            HttpRequestMessage request = new HttpRequestMessage
            {
                Content = content,
                Method = HttpMethod.Post,
                RequestUri = new Uri(requestUri)
            };

            HttpResponseMessage response = null;
            int counter = retryCount;

            try
            {
                response = await HttpClient.SendAsync(request);
            }
            catch { }
            while (response?.IsSuccessStatusCode != true && counter > 0)
            {
                await Task.Delay(delayMilliseconds);
                try
                {
                    --counter;
                    response = await HttpClient.SendAsync(request);
                }
                catch { }
            }
            UpdateWatchlistResult result = new UpdateWatchlistResult
            {
                HttpStatusCode = response?.StatusCode ?? HttpStatusCode.RequestTimeout
            };

            await ReadResponseAsStringIntoResultWhenSafe(result, response);

            return result;
        }

        // adds or removes media from favorite list
        // media type: "movie" OR "tv"
        //TODO refactor media type to enum
        public async Task<UpdateFavoriteListResult> UpdateFavoriteList(string mediaType, bool add, int mediaId, int? accountId = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            // no mistake here: missing "account_id" parameter add the string literal "{account_id}" as paths segment                        
            string baseUrl = BASE_Address + BASE_Path + ACCOUNT_DETAILS_Path + "/" + (accountId.HasValue ? accountId.Value.ToString() : "{account_id}") + FAVORITE_Path;

            var query = new Dictionary<string, string>();
            query.Add(API_KEY_Key, _settings.ApiKey);
            query.Add(SESSION_ID_Key, _settings.SessionId);

            string requestUri = QueryHelpers.AddQueryString(baseUrl, query);

            var jsonObj = new
            {
                media_type = mediaType,
                media_id = mediaId,
                favorite = add
            };

            string json = JsonConvert.SerializeObject(jsonObj);
            var content = new StringContent(json, encoding: Encoding.UTF8, mediaType: "application/json");

            HttpRequestMessage request = new HttpRequestMessage
            {
                Content = content,
                Method = HttpMethod.Post,
                RequestUri = new Uri(requestUri)
            };

            HttpResponseMessage response = null;
            int counter = retryCount;

            try
            {
                response = await HttpClient.SendAsync(request);
            }
            catch { }
            while (response?.IsSuccessStatusCode != true && counter > 0)
            {
                await Task.Delay(delayMilliseconds);
                try
                {
                    --counter;
                    response = await HttpClient.SendAsync(request);
                }
                catch { }
            }
            UpdateFavoriteListResult result = new UpdateFavoriteListResult
            {
                HttpStatusCode = response?.StatusCode ?? HttpStatusCode.RequestTimeout
            };

            await ReadResponseAsStringIntoResultWhenSafe(result, response);

            return result;
        }

        public async Task<RateMovieResult> RateMovie(Rating rating, int mediaId, string guestSessionId = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            if (!string.IsNullOrEmpty(guestSessionId))
                throw new NotImplementedException($"Rating with guest session is not supported by the method: {nameof(RateMovie)}, parameter: {nameof(guestSessionId)}={guestSessionId}");

            string baseUrl = BASE_Address + BASE_Path + MOVIE_DETAILS_Path + "/" + mediaId + RATING_Path;

            var query = new Dictionary<string, string>();
            query.Add(API_KEY_Key, _settings.ApiKey);
            query.Add(SESSION_ID_Key, _settings.SessionId);

            string requestUri = QueryHelpers.AddQueryString(baseUrl, query);

            var jsonObj = new
            {
                value = rating.GetValue()
            };

            string json = JsonConvert.SerializeObject(jsonObj);
            var content = new StringContent(json, encoding: Encoding.UTF8, mediaType: "application/json");

            HttpRequestMessage request = new HttpRequestMessage
            {
                Content = content,
                Method = HttpMethod.Post,
                RequestUri = new Uri(requestUri)
            };

            HttpResponseMessage response = null;
            int counter = retryCount;

            try
            {
                response = await HttpClient.SendAsync(request);
            }
            catch { }
            while (response?.IsSuccessStatusCode != true && counter > 0)
            {
                await Task.Delay(delayMilliseconds);
                try
                {
                    --counter;
                    response = await HttpClient.SendAsync(request);
                }
                catch { }
            }
            RateMovieResult result = new RateMovieResult
            {
                HttpStatusCode = response?.StatusCode ?? HttpStatusCode.RequestTimeout
            };

            await ReadResponseAsStringIntoResultWhenSafe(result, response);

            return result;
        }

        public async Task<DeleteMovieRatingResult> DeleteMovieRating(int mediaId, string guestSessionId = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            if (!string.IsNullOrEmpty(guestSessionId))
                throw new NotImplementedException($"Deleting rating with guest session is not supported by the method: {nameof(DeleteMovieRating)}, parameter: {nameof(guestSessionId)}={guestSessionId}");

            string baseUrl = BASE_Address + BASE_Path + MOVIE_DETAILS_Path + "/" + mediaId + RATING_Path;

            var query = new Dictionary<string, string>();
            query.Add(API_KEY_Key, _settings.ApiKey);
            query.Add(SESSION_ID_Key, _settings.SessionId);

            string requestUri = QueryHelpers.AddQueryString(baseUrl, query);

            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(requestUri)
            };

            HttpResponseMessage response = null;
            int counter = retryCount;

            try
            {
                response = await HttpClient.SendAsync(request);
            }
            catch { }
            while (response?.IsSuccessStatusCode != true && counter > 0)
            {
                await Task.Delay(delayMilliseconds);
                try
                {
                    --counter;
                    response = await HttpClient.SendAsync(request);
                }
                catch { }
            }
            DeleteMovieRatingResult result = new DeleteMovieRatingResult
            {
                HttpStatusCode = response?.StatusCode ?? HttpStatusCode.RequestTimeout
            };

            await ReadResponseAsStringIntoResultWhenSafe(result, response);

            return result;
        }


        public async Task<RemoveMovieResult> RemoveMovie(int listId, int mediaId, int retryCount = 0, int delayMilliseconds = 1000)
        {
            string baseUrl = BASE_Address + BASE_Path + LIST_path + "/" + listId + REMOVE_MEDIA_Path;

            var query = new Dictionary<string, string>();
            query.Add(API_KEY_Key, _settings.ApiKey);
            query.Add(SESSION_ID_Key, _settings.SessionId);

            string requestUri = QueryHelpers.AddQueryString(baseUrl, query);

            var jsonObj = new
            {
                media_id = mediaId
            };

            string json = JsonConvert.SerializeObject(jsonObj);
            var content = new StringContent(json, encoding: Encoding.UTF8, mediaType: "application/json");

            HttpRequestMessage request = new HttpRequestMessage
            {
                Content = content,
                Method = HttpMethod.Post,
                RequestUri = new Uri(requestUri)
            };

            HttpResponseMessage response = null;
            int counter = retryCount;

            try
            {
                response = await HttpClient.SendAsync(request);
            }
            catch { }
            while (response?.IsSuccessStatusCode != true && counter > 0)
            {
                await Task.Delay(delayMilliseconds);
                try
                {
                    --counter;
                    response = await HttpClient.SendAsync(request);
                }
                catch { }
            }

            RemoveMovieResult result = new RemoveMovieResult
            {
                HttpStatusCode = response?.StatusCode ?? HttpStatusCode.RequestTimeout
            };

            await ReadResponseAsStringIntoResultWhenSafe(result, response);

            return result;
        }

        private async Task<T> GetResponse<T>(int retryCount, int delayMilliseconds, string requestUri) where T : TmdbResponseBase, new()
        {
            HttpResponseMessage response = null;
            int counter = retryCount;

            try
            {
                response = await HttpClient.GetAsync(requestUri);
            }
            catch { }
            while (response?.IsSuccessStatusCode != true && counter > 0)
            {
                await Task.Delay(delayMilliseconds);

                try
                {
                    --counter;
                    response = await HttpClient.GetAsync(requestUri);
                }
                catch { }
            }
            T result = new T { HttpStatusCode = response?.StatusCode ?? HttpStatusCode.RequestTimeout };

            await ReadResponseAsStringIntoResultWhenSafe(result, response);

            return result;
        }

        private async Task ReadResponseAsStringIntoResultWhenSafe<T>(T result, HttpResponseMessage response) where T : TmdbResponseBase
        {
            try
            {
                if (result.HttpStatusCode.IsSuccessCode())
                    result.Json = await response.Content.ReadAsStringAsync();
            }
            catch
            {
                result.HttpStatusCode = HttpStatusCode.RequestTimeout;
            }
        }
    }
}

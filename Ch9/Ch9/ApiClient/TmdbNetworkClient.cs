using Ch9.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using System.Net;
using static Ch9.ApiClient.WebApiPathConstants;
using Newtonsoft.Json;
using System.Text;

namespace Ch9.ApiClient
{
    // TODO : SessionId should be injected into the public methods via parameter instead of grabbing it from ISettings
    // This will help with caching (SessionId needs to be used as part of the caching key to differentiate between users)
    public class TmdbNetworkClient : ITmdbNetworkClient
    {
        private readonly ISettings _settings;
        private static Lazy<HttpClient> httpClient = new Lazy<HttpClient>(
            () =>
            {
                var client = new HttpClient();
                return client;
            });

        private HttpClient HttpClient => httpClient.Value;

        public TmdbNetworkClient(ISettings settings)
        {
            _settings = settings;
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
        public async Task<SearchByMovieResult> SearchByMovie(string searchString, string language = null, bool? includeAdult = null, int? page = null, int retryCount = 0, int delayMilliseconds = 1000)
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

            string requestUri = QueryHelpers.AddQueryString(baseUrl, query);

            SearchByMovieResult result = await GetResponse<SearchByMovieResult>(retryCount, delayMilliseconds, requestUri);

            return result;
        }
        //TODO : Make search option to enum 
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

        // Fetches movie details, swallows exceptions
        // retries as needed
        public async Task<FetchMovieDetailsResult> FetchMovieDetails(int id, string language = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            string baseUrl = BASE_Address + BASE_Path + MOVIE_DETAILS_Path + "/" + id;

            var query = new Dictionary<string, string>();
            query.Add(API_KEY_Key, _settings.ApiKey);

            if (!string.IsNullOrEmpty(language))
                query.Add(LANGUAGE_Key, language);

            string requestUri = QueryHelpers.AddQueryString(baseUrl, query);

            FetchMovieDetailsResult result = await GetResponse<FetchMovieDetailsResult>(retryCount, delayMilliseconds, requestUri);

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

        // Fetches the image paths of the gallery images from the TMDB server
        // swallows exceptions, retries as required
        public async Task<GetMovieImagesResult> UpdateMovieImages(int id, string language = null, string otherLanguage = null, bool? includeLanguageless = true, int retryCount = 0, int delayMilliseconds = 1000)
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

        public async Task<GetMovieRecommendationsResult> GetMovieRecommendations(int id, string language = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            string baseUrl = BASE_Address + BASE_Path + MOVIE_DETAILS_Path + "/" + id + RECOMMENDATIONS_Path;

            var query = new Dictionary<string, string>();
            query.Add(API_KEY_Key, _settings.ApiKey);

            if (!string.IsNullOrEmpty(language))
                query.Add(LANGUAGE_Key, language);

            string requestUri = QueryHelpers.AddQueryString(baseUrl, query);

            GetMovieRecommendationsResult result = await GetResponse<GetMovieRecommendationsResult>(retryCount, delayMilliseconds, requestUri);

            return result;
        }

        public async Task<GetSimilarMoviesResult> GetSimilarMovies(int id, string language = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            string baseUrl = BASE_Address + BASE_Path + MOVIE_DETAILS_Path + "/" + id + SIMILARS_Path;

            var query = new Dictionary<string, string>();
            query.Add(API_KEY_Key, _settings.ApiKey);

            if (!string.IsNullOrEmpty(language))
                query.Add(LANGUAGE_Key, language);

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
            if (response.IsSuccessStatusCode)
                result.Json = await response.Content.ReadAsStringAsync();
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
            if (response.IsSuccessStatusCode)
                result.Json = await response.Content.ReadAsStringAsync();
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

            if (response.IsSuccessStatusCode)
                result.Json = await response.Content.ReadAsStringAsync();
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

            if (response.IsSuccessStatusCode)
                result.Json = await response.Content.ReadAsStringAsync();
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

            result.Json = await response.Content.ReadAsStringAsync();
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

            if (response.IsSuccessStatusCode)
                result.Json = await response.Content.ReadAsStringAsync();
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

            if (response.IsSuccessStatusCode)
                result.Json = await response.Content.ReadAsStringAsync();
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

            if (response.IsSuccessStatusCode)
                result.Json = await response.Content.ReadAsStringAsync();
            return result;
        }


    }
}

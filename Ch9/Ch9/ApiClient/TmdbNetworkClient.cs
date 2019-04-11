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
            this._settings = settings;
            settings.ApiKey = settings.ApiKey;
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

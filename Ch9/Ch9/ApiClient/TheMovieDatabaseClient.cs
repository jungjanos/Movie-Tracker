using Ch9.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using System.Net;
using static Ch9.ApiClient.WebApiPathConstants;

namespace Ch9.ApiClient
{
    public class TheMovieDatabaseClient
    {
        private string apiKeyValue;
        private readonly Settings settings;
        private static Lazy<HttpClient> httpClient = new Lazy<HttpClient>(
            () =>
            {
                var client = new HttpClient();
                return client;
            });

        private HttpClient HttpClient => httpClient.Value;

        public TheMovieDatabaseClient(Settings settings)
        {
            this.settings = settings;
            apiKeyValue = settings.ApiKey;
        }

        public class TmdbConfigurationModelResult
        {
            public HttpStatusCode HttpStatusCode;
            public string Json { get; set; }
        }

        public async Task<TmdbConfigurationModelResult> GetTmdbConfiguration(int retryCount = 0, int delayMilliseconds = 1000)
        {
            string baseUrl = BASE_Address + BASE_Path + CONFIG_Path;

            var query = new Dictionary<string, string>();
            query.Add(API_KEY_Key, apiKeyValue);

            HttpResponseMessage response = null;
            int counter = retryCount;

            try
            {
                response = await HttpClient.GetAsync(QueryHelpers.AddQueryString(baseUrl, query));
            }
            catch { }
            while (response?.IsSuccessStatusCode != true && counter > 0)
            {
                await Task.Delay(delayMilliseconds);

                try
                {
                    response = await HttpClient.GetAsync(QueryHelpers.AddQueryString(baseUrl, query));
                }
                catch { }
            }

            TmdbConfigurationModelResult result = new TmdbConfigurationModelResult { HttpStatusCode = response?.StatusCode ?? HttpStatusCode.RequestTimeout };

            if (response.IsSuccessStatusCode)
                result.Json = await response.Content.ReadAsStringAsync();

            return result;
        }

        public class GenreNameFetchResult
        {
            public HttpStatusCode HttpStatusCode { get; set; }
            public string Json { get; set; }
        }

        // Fetches Genre information from WebAPI
        // indicates success or failure as Http code,
        // retries as required
        // ToDo: magic string (language should be replaced by enum type) 
        public async Task<GenreNameFetchResult> FetchGenreIdsWithNames(string language = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            string baseUrl = BASE_Address + BASE_Path + GENRE_LIST_Path;

            var query = new Dictionary<string, string>();
            query.Add(API_KEY_Key, apiKeyValue);

            if (!string.IsNullOrEmpty(language))
                query.Add(LANGUAGE_Key, language);

            HttpResponseMessage response = null;
            int counter = retryCount;

            try
            {
                response = await HttpClient.GetAsync(QueryHelpers.AddQueryString(baseUrl, query));
            }
            catch { }
            while (response?.IsSuccessStatusCode != true && counter > 0)
            {
                await Task.Delay(delayMilliseconds);

                try
                {
                    response = await HttpClient.GetAsync(QueryHelpers.AddQueryString(baseUrl, query));
                }
                catch { }
            }

            GenreNameFetchResult result = new GenreNameFetchResult { HttpStatusCode = response?.StatusCode ?? HttpStatusCode.RequestTimeout };

            if (response.IsSuccessStatusCode)
                result.Json = await response.Content.ReadAsStringAsync();

            return result;
        }

        public class SearchResult
        {
            public HttpStatusCode HttpStatusCode { get; set; }

            [JsonProperty("page")]
            public int Page { get; set; }

            [JsonProperty("results")]
            public List<MovieDetailModel> MovieDetailModels { get; set; }

            [JsonProperty("total_results")]
            public int TotalResults { get; set; }

            [JsonProperty("total_pages")]
            public int TotalPages { get; set; }
        }

        public class SearchByMovieResult
        {
            public HttpStatusCode HttpStatusCode { get; set; }
            public string Json { get; set; }
        }

        // Searches for moves according to settings 
        // Swallows exceptions retries as needed    
        public async Task<SearchByMovieResult> SearchByMovie(string searchString, string language = null, bool? includeAdult = null, int? page = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            string baseUrl = BASE_Address + BASE_Path + SEARCH_MOVIE_Path;

            var query = new Dictionary<string, string>();
            query.Add(API_KEY_Key, apiKeyValue);
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

            HttpResponseMessage response = null;
            int counter = retryCount;

            try
            {
                response = await HttpClient.GetAsync(QueryHelpers.AddQueryString(baseUrl, query));
            }
            catch { }
            while (response?.IsSuccessStatusCode != true && counter > 0)
            {
                await Task.Delay(delayMilliseconds);

                try
                {
                    response = await HttpClient.GetAsync(QueryHelpers.AddQueryString(baseUrl, query));
                }
                catch { }
            }
            SearchByMovieResult result = new SearchByMovieResult { HttpStatusCode = response?.StatusCode ?? HttpStatusCode.RequestTimeout };

            if (response.IsSuccessStatusCode)
                result.Json = await response.Content.ReadAsStringAsync();

            return result;
        }

        public class FetchMovieDetailsResult
        {
            public HttpStatusCode HttpStatusCode { get; set; }
            public string Json { get; set; }
        }

        // Fetches movie details, swallows exceptions
        // retries as needed
        public async Task<FetchMovieDetailsResult> FetchMovieDetails(int id, string language = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            string baseUrl = BASE_Address + BASE_Path + MOVIE_DETAILS_Path + "/" + id;

            var query = new Dictionary<string, string>();
            query.Add(API_KEY_Key, apiKeyValue);

            if (!string.IsNullOrEmpty(language))
                query.Add(LANGUAGE_Key, language);

            HttpResponseMessage response = null;
            int counter = retryCount;

            try
            {
                response = await HttpClient.GetAsync(QueryHelpers.AddQueryString(baseUrl, query));
            }
            catch { }
            while (response?.IsSuccessStatusCode != true && counter > 0)
            {
                await Task.Delay(delayMilliseconds);

                try
                {
                    response = await HttpClient.GetAsync(QueryHelpers.AddQueryString(baseUrl, query));
                }
                catch { }
            }
            FetchMovieDetailsResult result = new FetchMovieDetailsResult { HttpStatusCode = response?.StatusCode ?? HttpStatusCode.RequestTimeout };

            if (response.IsSuccessStatusCode)
                result.Json = await response.Content.ReadAsStringAsync();

            return result;
        }

        // The two classes use the same result format,
        // inheritacne is used only to give a more descriptive class name
        public class TrendingMoviesResult : SearchByMovieResult { }

        // Fetches the list of trending moves according to query
        // swallows exceptions, retries as configured
        public async Task<TrendingMoviesResult> GetTrendingMovies(bool week = true, string language = null, bool? includeAdult = null, int? page = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            string baseUrl = BASE_Address + BASE_Path + TRENDING_Path + TRENDING_MOVIE_Selector + (week ? WEEK_Path : DAY_Path);

            var query = new Dictionary<string, string>();
            query.Add(API_KEY_Key, apiKeyValue);

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

            HttpResponseMessage response = null;
            int counter = retryCount;

            try
            {
                response = await HttpClient.GetAsync(QueryHelpers.AddQueryString(baseUrl, query));
            }
            catch { }
            while (response?.IsSuccessStatusCode != true && counter > 0)
            {
                await Task.Delay(delayMilliseconds);

                try
                {
                    response = await HttpClient.GetAsync(QueryHelpers.AddQueryString(baseUrl, query));
                }
                catch { }
            }
            TrendingMoviesResult result = new TrendingMoviesResult { HttpStatusCode = response?.StatusCode ?? HttpStatusCode.RequestTimeout };

            if (response.IsSuccessStatusCode)
                result.Json = await response.Content.ReadAsStringAsync();

            return result;
        }

        public class GetMovieImagesResult
        {
            public HttpStatusCode HttpStatusCode { get; set; }
            public string Json { get; set; }
        }

        // Fetches the image paths of the gallery images from the server
        // swallows exceptions, retries as required
        public async Task<GetMovieImagesResult> UpdateMovieImages2(int id, string language = null, string otherLanguage = null, bool? includeLanguageless = true, int retryCount = 0, int delayMilliseconds = 1000)
        {
            string baseUrl = BASE_Address + BASE_Path + MOVIE_DETAILS_Path + "/" + id + IMAGE_DETAIL_Path;
            var query = new Dictionary<string, string>();
            query.Add(API_KEY_Key, apiKeyValue);

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

            HttpResponseMessage response = null;
            int counter = retryCount;

            try
            {
                response = await HttpClient.GetAsync(QueryHelpers.AddQueryString(baseUrl, query));
            }
            catch { }
            while (response?.IsSuccessStatusCode != true && counter > 0)
            {
                await Task.Delay(delayMilliseconds);

                try
                {
                    response = await HttpClient.GetAsync(QueryHelpers.AddQueryString(baseUrl, query));
                }
                catch { }
            }
            GetMovieImagesResult result = new GetMovieImagesResult { HttpStatusCode = response?.StatusCode ?? HttpStatusCode.RequestTimeout };

            if (response.IsSuccessStatusCode)
                result.Json = await response.Content.ReadAsStringAsync();

            return result;
        }
    }
}

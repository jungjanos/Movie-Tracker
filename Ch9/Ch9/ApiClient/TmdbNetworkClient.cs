using Ch9.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using System.Net;
using static Ch9.ApiClient.WebApiPathConstants;

namespace Ch9.ApiClient
{
    public class TmdbNetworkClient
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

        public TmdbNetworkClient(Settings settings)
        {
            this.settings = settings;
            apiKeyValue = settings.ApiKey;
        }

        public async Task<TmdbConfigurationModelResult> GetTmdbConfiguration(int retryCount = 0, int delayMilliseconds = 1000)
        {
            string baseUrl = BASE_Address + BASE_Path + CONFIG_Path;

            var query = new Dictionary<string, string>();
            query.Add(API_KEY_Key, apiKeyValue);

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
            query.Add(API_KEY_Key, apiKeyValue);

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
            query.Add(API_KEY_Key, apiKeyValue);

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

            string requestUri = QueryHelpers.AddQueryString(baseUrl, query);

            TrendingMoviesResult result = await GetResponse<TrendingMoviesResult>(retryCount, delayMilliseconds, requestUri);

            return result;
        }


        // Fetches the image paths of the gallery images from the server
        // swallows exceptions, retries as required
        public async Task<GetMovieImagesResult> UpdateMovieImages(int id, string language = null, string otherLanguage = null, bool? includeLanguageless = true, int retryCount = 0, int delayMilliseconds = 1000)
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

            string requestUri = QueryHelpers.AddQueryString(baseUrl, query);
            GetMovieImagesResult result = await GetResponse<GetMovieImagesResult>(retryCount, delayMilliseconds, requestUri);

            return result;
        }


        private async Task<T> GetResponse<T>(int retryCount, int delayMilliseconds, string requestUri) where T: TmdbResponseBase, new()
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

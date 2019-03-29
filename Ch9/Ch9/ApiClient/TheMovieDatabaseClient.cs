using Ch9.Models;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using System.Net;

namespace Ch9.ApiClient
{
    public class TheMovieDatabaseClient
    {
        private const string BASE_Addr = "https://api.themoviedb.org";
        private const string BASE_Path = "/3";
        private const string CONFIG_Path = "/configuration";
        private const string GENRE_List_Path = "/genre/movie/list";
        private const string SEARCH_Movie_Path = "/search/movie";
        private const string SEARCH_Query_Key = "query";
        private const string TRENDING_Path = "/trending";
        private const string TRENDING_Movie_type_selector = "/movie";
        private const string WEEK_Path = "/week";
        private const string DAY_Path = "/day";
        private const string IMAGE_Detail_Path = "/images";
        private const string IMAGE_Additional_Languages = "include_image_language";

        private const string TRENDING_Week_Path = "/trending/movie/week";
        private const string TRENDING_Day_Path = "/trending/movie/day";
        private const string MOVIE_Details_Path = "/movie";

        private Func<int, string> GET_Movie_Images_Path = (int Id) => ("/movie/" + Id + "/images");
        private const string LANGUAGE_Key = "language";
        private const string PAGE_Key = "page";
        private const string INCLUDE_Adult_Key = "include_adult";


        private const string API_KEY_KEY = "api_key";
        private const string API_KEY_Value = "764d596e888359d26c0dc49deffecbb3";

        private static Lazy<HttpClient> httpClient = new Lazy<HttpClient>(
            () =>
            {
                var client = new HttpClient();
                return client;
            });        

        private HttpClient HttpClient => httpClient.Value;

        public class TmdbConfigurationModelResult
        {
            public HttpStatusCode HttpStatusCode;
            public string Json { get; set; }
        }

        public async Task<TmdbConfigurationModelResult> GetTmdbConfiguration(int retryCount = 0, int delayMilliseconds = 1000)
        {
            string baseUrl = BASE_Addr + BASE_Path + CONFIG_Path;

            var query = new Dictionary<string, string>();
            query.Add(API_KEY_KEY, API_KEY_Value);

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
            public GenreIdNamePair[] IdNamePairs { get; set; }
            public string Json { get; set; }
        }

        // Fetches Genre information from WebAPI
        // indicates success or failure as Http code,
        // retries as required
        // ToDo: magic string (language should be replaced by enum type) 
        public async Task<GenreNameFetchResult> FetchGenreIdsWithNames(string language = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            string baseUrl = BASE_Addr + BASE_Path + GENRE_List_Path;

            var query = new Dictionary<string, string>();
            query.Add(API_KEY_KEY, API_KEY_Value);

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
                //{
                //    var message = await response.Content.ReadAsStringAsync();
                //    GenreIdNamePairWrapper responseObject = JsonConvert.DeserializeObject<GenreIdNamePairWrapper>(message);
                //    result.IdNamePairs = responseObject.Genres;
                //}
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
            string baseUrl = BASE_Addr + BASE_Path + SEARCH_Movie_Path;

            var query = new Dictionary<string, string>();
            query.Add(API_KEY_KEY, API_KEY_Value);
            query.Add(SEARCH_Query_Key, searchString);

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
            string baseUrl = BASE_Addr + BASE_Path + MOVIE_Details_Path + "/" + id;

            var query = new Dictionary<string, string>();
            query.Add(API_KEY_KEY, API_KEY_Value);

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
            string baseUrl = BASE_Addr + BASE_Path + TRENDING_Path + TRENDING_Movie_type_selector + (week ? WEEK_Path : DAY_Path);

            var query = new Dictionary<string, string>();
            query.Add(API_KEY_KEY, API_KEY_Value);

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
            string baseUrl = BASE_Addr + BASE_Path + MOVIE_Details_Path + "/" + id + IMAGE_Detail_Path;
            var query = new Dictionary<string, string>();
            query.Add(API_KEY_KEY, API_KEY_Value);

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
                query.Add(IMAGE_Additional_Languages, otherLanguagesValue);

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

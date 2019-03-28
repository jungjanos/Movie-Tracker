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

        public class MovieDetailsUpdateResult
        {
            public HttpStatusCode StatusCode { get; set; }
        }

        public class GetMovieImagesResult
        {
            public HttpStatusCode HttpStatusCode { get; set; }
        }

        




        private const string BASE_Addr = "https://api.themoviedb.org";
        private const string BASE_Path = "/3";
        private const string CONFIG_Path = "/configuration";
        private const string GENRE_List_Path = "/genre/movie/list";
        private const string SEARCH_Movie_Path = "/search/movie";
        private const string SEARCH_Query_Key = "query";
        private const string TRENDING_Week_Path = "/trending/movie/week";
        private const string TRENDING_Day_Path = "/trending/movie/day";
        private const string MOVIE_Details_Path = "/movie";
        private Func<int, string> GET_Movie_Images_Path = (int Id) => ("/movie/" + Id + "/images");
        private const string LANGUAGE_Selector = "language";


        private const string API_KEY_KEY = "api_key";
        private const string API_KEY_Value = "764d596e888359d26c0dc49deffecbb3";

        private static Lazy<HttpClient> httpClient = new Lazy<HttpClient>(
            () =>
            {
                var client = new HttpClient();
                return client;
            });

        private Dictionary<int, string> genreIdNamePairs;


        private HttpClient HttpClient => httpClient.Value;


        public TmdbConfigurationModel ConfigurationModel { get; private set; }


        public async Task InitializeConfigurationAsync()
        {
            await Task.WhenAll(GetGenreIdNamePairsAsync(), GetSettingsAsync());

            // OLD NEEDS TO BE REPLACED!!!
            async Task GetGenreIdNamePairsAsync()
            {
                UriBuilder uriBuilder = new UriBuilder(new Uri(BASE_Addr));
                uriBuilder.Path = BASE_Path + GENRE_List_Path;
                uriBuilder.Query = API_KEY_KEY + "=" + API_KEY_Value;

                try
                {
                    var genreResponse = await HttpClient.GetStringAsync(uriBuilder.Uri);
                    var genreList = JsonConvert.DeserializeObject<GenreIdNamePairWrapper>(genreResponse).Genres;

                    genreIdNamePairs = new Dictionary<int, string>();

                    foreach (var genre in genreList)
                        genreIdNamePairs[genre.Id] = genre.Name;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }

            async Task GetSettingsAsync()
            {
                UriBuilder uriBuilder = new UriBuilder(new Uri(BASE_Addr));
                uriBuilder.Path = BASE_Path + CONFIG_Path;
                uriBuilder.Query = API_KEY_KEY + "=" + API_KEY_Value;

                var configResponse = await HttpClient.GetStringAsync(uriBuilder.Uri);

                ConfigurationModel = JsonConvert.DeserializeObject<TmdbConfigurationModel>(configResponse);
            }
        }

        public async Task<SearchResult> SearchByMovie(string searchString, int page = 0)
        {
            string baseUrl = BASE_Addr + BASE_Path + SEARCH_Movie_Path;

            var query = new Dictionary<string, string>();
            query.Add(API_KEY_KEY, API_KEY_Value);
            query.Add(SEARCH_Query_Key, searchString);
            query.Add("include_adult", "false");
            if (page > 0)
                query.Add("page", page.ToString());

            var response = await HttpClient.GetAsync(QueryHelpers.AddQueryString(baseUrl, query));
            SearchResult searchResult = new SearchResult()
            {
                HttpStatusCode = response.StatusCode,
                MovieDetailModels = new List<MovieDetailModel>()
            };

            if (response.StatusCode == HttpStatusCode.OK)
            {
                string content = await response.Content.ReadAsStringAsync();
                searchResult.MovieDetailModels = JsonConvert.DeserializeObject<SearchResult>(content).MovieDetailModels
                                                .Where(x => x.ReleaseDate > DateTime.Today - TimeSpan.FromDays(365 * 30))
                                                .Where(x => x.ReleaseDate?.Year <= DateTime.Today.Year).
                                                Where(x => x.GenreIds != null && x.GenreIds.Length != 0)
                                                .ToList();

                SetGenreNamesFromGenreIds(searchResult.MovieDetailModels);
                SetImageSrc(searchResult.MovieDetailModels);
            }

            return searchResult;
        }

        public async Task<SearchResult> GetTrending(bool week = true)
        {
            string baseUrl = BASE_Addr + BASE_Path + (week ? TRENDING_Week_Path : TRENDING_Day_Path);

            var query = new Dictionary<string, string>();
            query.Add(API_KEY_KEY, API_KEY_Value);

            var response = await HttpClient.GetAsync(QueryHelpers.AddQueryString(baseUrl, query));
            SearchResult searchResult = new SearchResult()
            {
                HttpStatusCode = response.StatusCode,
                MovieDetailModels = new List<MovieDetailModel>()
            };

            if (response.StatusCode == HttpStatusCode.OK)
            {
                string content = await response.Content.ReadAsStringAsync();
                searchResult.MovieDetailModels = JsonConvert.DeserializeObject<SearchResult>(content).MovieDetailModels
                                                .Where(x => x.ReleaseDate > DateTime.Today - TimeSpan.FromDays(365 * 30))
                                                .Where(x => x.ReleaseDate?.Year <= DateTime.Today.Year).
                                                Where(x => x.GenreIds != null && x.GenreIds.Length != 0)
                                                .ToList();

                SetGenreNamesFromGenreIds(searchResult.MovieDetailModels);
                SetImageSrc(searchResult.MovieDetailModels);
            }
            return searchResult;
        }


        //Potential race condition via side effects when reentering function!!!!
        public async Task<MovieDetailsUpdateResult> UpdateMovieDetail(MovieDetailModel movie)
        {
            string baseUrl = BASE_Addr + BASE_Path + MOVIE_Details_Path + "/" + movie.Id;
            var query = new Dictionary<string, string>();
            query.Add(API_KEY_KEY, API_KEY_Value);
            var response = await HttpClient.GetAsync(QueryHelpers.AddQueryString(baseUrl, query));
            MovieDetailsUpdateResult movieDetailsUpdateResult = new MovieDetailsUpdateResult()
            {
                StatusCode = response.StatusCode
            };

            if (response.StatusCode == HttpStatusCode.OK)
            {
                string content = await response.Content.ReadAsStringAsync();
                JsonConvert.PopulateObject(content, movie);
            }
            return movieDetailsUpdateResult;
        }

        //Potential race condition via side effects when reentering function!!!!
        public async Task<GetMovieImagesResult> UpdateMovieImages(MovieDetailModel movie)
        {
            string baseUrl = BASE_Addr + BASE_Path + GET_Movie_Images_Path(movie.Id);
            var query = new Dictionary<string, string>();
            query.Add(API_KEY_KEY, API_KEY_Value);
            query.Add("language", "en,null"); 

            var response = await HttpClient.GetAsync(QueryHelpers.AddQueryString(baseUrl, query));

            GetMovieImagesResult getMovieImagesResult = new GetMovieImagesResult()
            {
                HttpStatusCode = response.StatusCode
            };

            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (movie.ImageDetailCollection == null)
                    movie.ImageDetailCollection = new ImageDetailCollection();                

                string content = await response.Content.ReadAsStringAsync();
                JsonConvert.PopulateObject(content, movie.ImageDetailCollection);                
            }
            SetGalleryImageSources(movie);

            return getMovieImagesResult;
        }

        private void SetGalleryImageSources(MovieDetailModel movie)
        {
            List<string> tempResult = new List<string>(1 + movie.ImageDetailCollection.Backdrops?.Length ?? 0);

            tempResult.Add(movie.ImgBackdropSrc);

            if (movie.ImageDetailCollection.Posters?.Length > 0)
                tempResult.Add(ConfigurationModel.Images.BaseUrl + "w780" + movie.ImageDetailCollection.Posters.First().FilePath);

            if (movie.ImageDetailCollection.Backdrops?.Length > 0)
                foreach (ImageModel backdrop in movie.ImageDetailCollection.Backdrops)
                    tempResult.Add(ConfigurationModel.Images.BaseUrl + "w780" + backdrop.FilePath);

            movie.GalleryDisplayImages = tempResult.ToArray();
        }

        private void SetGenreNamesFromGenreIds(IEnumerable<MovieDetailModel> movies)
        {
            foreach (MovieDetailModel movie in movies)
            {
                movie.Genre = movie.GenreIds?.Length == null ? null :
                    string.Join(", ", movie.GenreIds.Select(id => genreIdNamePairs[id]))
                    .TrimEnd(new char[] { ',', ' ' });
            }
        }

        private void SetImageSrc(IEnumerable<MovieDetailModel> movies)
        {
            foreach (MovieDetailModel movie in movies)
            {
                movie.ImgSmSrc = ConfigurationModel.Images.BaseUrl + ConfigurationModel.Images.PosterSizes[0] + movie.ImgPosterName;
                movie.ImgBackdropSrc = ConfigurationModel.Images.BaseUrl + "w780" + movie.ImgBackdropName;
                movie.GalleryDisplayImages = new string[] 
                {
                    ConfigurationModel.Images.BaseUrl + "w780" + movie.ImgBackdropName
                };
                movie.GalleryDisplayImage = movie.GalleryDisplayImages[0];            
            }
        }



        // NEW REFACTORED CODE AFTER THIS LINE OLD CODE ABOVE

        public class GenreNameFetchResult
        {
            public HttpStatusCode HttpStatusCode { get; set; }
            public GenreIdNamePair[] IdNamePairs { get; set; }
        }


        // Fetches Genre information from WebAPI
        // indicates success or failure as Http code,
        // retries as required
        // ToDo: magic string (language should be replaced by enum type) 
        public async Task<GenreNameFetchResult> FetchGenreIdsWithNames(string language = null,  int retryCount=0, int delayMilliseconds = 1000)
        {
            string baseUrl = BASE_Addr + BASE_Path + GENRE_List_Path;

            var query = new Dictionary<string, string>();
            query.Add(API_KEY_KEY, API_KEY_Value);

            if (!string.IsNullOrEmpty(language))
                query.Add(LANGUAGE_Selector, language);


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
            {
                var message = await response.Content.ReadAsStringAsync();
                GenreIdNamePairWrapper responseObject = JsonConvert.DeserializeObject<GenreIdNamePairWrapper>(message);
                result.IdNamePairs = responseObject.Genres;
            }
            return result;
        }


    }
}

using Ch9.ApiClient;
using Ch9.Utils;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using LazyCache;
using System.Threading.Tasks;
using Ch9.Models;
using Newtonsoft.Json;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Ch9
{
    public partial class App : Application
    {
        public Settings Settings { get; private set; }
        public MovieGenreSettings MovieGenreSettings { get; private set; }
        public TmdbConfigurationModel TmdbConfiguration { get; private set; }
        public MovieDetailModelConfigurator MovieDetailModelConfigurator { get; private set; }
        public SearchResultFilter ResultFilter { get; private set; }


        public TheMovieDatabaseClient ApiClient { get; private set; } 
        public IAppCache MovieSearchCache { get; } = new CachingService();
        
        public Func<string, string, bool?, Task<TheMovieDatabaseClient.SearchByMovieResult>> movieGetter2;

        //(bool week = true, string language = null, bool? includeAdult = null, int? page = null, 
        // int retryCount = 0, int delayMilliseconds = 1000)
        public Func<bool, string, bool?, int?, Task<TheMovieDatabaseClient.TrendingMoviesResult>> TrendingMoviesGetter;

        public Func<int, string, Task<TheMovieDatabaseClient.FetchMovieDetailsResult>> MovieDetailGetter;

        // public async Task<GetMovieImagesResult2> UpdateMovieImages2(int id, string language = null, 
        // string otherLanguage = null, bool? includeLanguageless = true, int retryCount = 0, int delayMilliseconds = 1000)
        public Func<int, string, string, bool?, Task<TheMovieDatabaseClient.GetMovieImagesResult>> GetMovieImages;

        public App()
        {            
            Settings = new Settings();
            ApiClient = new TheMovieDatabaseClient(Settings);
            MovieGenreSettings = new MovieGenreSettings();
            ResultFilter = new SearchResultFilter(Settings, MovieGenreSettings);

            movieGetter2 = (string searchString, string searchLanguage, bool? includeAdult) =>
            {
                return MovieSearchCache.GetOrAddAsync("$search: " + searchString + (searchLanguage ?? "") + (includeAdult?.ToString() ?? "") , () => ApiClient.SearchByMovie(searchString, searchLanguage, includeAdult));
            };        

            TrendingMoviesGetter = (week, language, includeAdult, page) =>
            {
                return MovieSearchCache.GetOrAddAsync("$trending: " + week  + (language ?? "") + (includeAdult?.ToString() ?? "") + (page?.ToString() ?? ""), () =>
                ApiClient.GetTrendingMovies(week, language, includeAdult));                
            };

            MovieDetailGetter = (int id, string language) =>
            {
                return MovieSearchCache.GetOrAddAsync("$MovieDetailGetter id=" + id.ToString() + (language ?? ""), () => ApiClient.FetchMovieDetails(id, language));
            };

            GetMovieImages = (int id, string language, string otherLanguage, bool? includeLanguageless) =>
            {
                return MovieSearchCache.GetOrAddAsync("$GetMovieImages id=" + id.ToString() + (language ?? "") + (otherLanguage ?? "") + (includeLanguageless == null ? "" : includeLanguageless.Value.ToString()), 
                    
                    () => ApiClient.UpdateMovieImages2(id, language, otherLanguage, includeLanguageless));
            };

            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
        }

        protected override async void OnStart()
        {
            //await ApiClient.InitializeConfigurationAsync();
            TmdbConfiguration = await GetTmdbConfiguration(3, 1000);
            MovieDetailModelConfigurator = new MovieDetailModelConfigurator(TmdbConfiguration, MovieGenreSettings);
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        private async Task<TmdbConfigurationModel> GetTmdbConfiguration(int retries, int retryDelay)
        {
            var response = await ApiClient.GetTmdbConfiguration(retries, retryDelay);

            if (200 <= (int)response.HttpStatusCode && (int)response.HttpStatusCode < 300)
            {
                var result = JsonConvert.DeserializeObject<TmdbConfigurationModel>(response.Json);
                return result;
            }
            else throw new TimeoutException($"Could not connect TMDB Server to fetch configuration data at application start, retried {retries}-times");
        }
    }
}

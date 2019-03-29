using Ch9.ApiClient;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using LazyCache;
using System.Threading.Tasks;
using Ch9.Models;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Ch9
{
    public partial class App : Application
    {
        public Settings Settings { get; private set; }
        public MovieGenreSettings MovieGenreSettings { get; private set; }

        public TheMovieDatabaseClient ApiClient { get; } = new TheMovieDatabaseClient();
        public IAppCache MovieSearchCache { get; } = new CachingService();
        
        public Func<string, string, bool?, Task<TheMovieDatabaseClient.SearchByMovieResult>> movieGetter2;
        public Func<Task<TheMovieDatabaseClient.SearchResult>> trendingWeekGetter;
        public Func<Task<TheMovieDatabaseClient.SearchResult>> trendingDayGetter;
        
        public Func<int, string, Task<TheMovieDatabaseClient.FetchMovieDetailsResult>> MovieDetailGetter;


        public App()
        {            
            Settings = new Settings();
            MovieGenreSettings = new MovieGenreSettings();

            movieGetter2 = (string searchString, string searchLanguage, bool? includeAdult) =>
            {
                return MovieSearchCache.GetOrAddAsync(searchString, () => ApiClient.SearchByMovie2(searchString, searchLanguage, includeAdult));
            };

            trendingWeekGetter = () =>
            {
                return MovieSearchCache.GetOrAddAsync("$week", () => ApiClient.GetTrending(true));
            };

            trendingDayGetter = () =>
            {
                return MovieSearchCache.GetOrAddAsync("$day", () => ApiClient.GetTrending(false));
            };

            MovieDetailGetter = (int id, string language) =>
            {
                return MovieSearchCache.GetOrAddAsync("$MovieDetailGetter id=" + id.ToString() + language, () => ApiClient.FetchMovieDetails(id, language));
            };



            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
        }

        protected override async void OnStart()
        {
            await ApiClient.InitializeConfigurationAsync();            
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}

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
        public Func<string, Task<TheMovieDatabaseClient.SearchResult>> movieGetter;
        public Func<Task<TheMovieDatabaseClient.SearchResult>> trendingWeekGetter;
        public Func<Task<TheMovieDatabaseClient.SearchResult>> trendingDayGetter;
        public Func<MovieDetailModel, Task<TheMovieDatabaseClient.MovieDetailsUpdateResult>> movieModelDeatilUpdateGetter;


        public App()
        {            
            Settings = new Settings();
            MovieGenreSettings = new MovieGenreSettings();

            movieGetter = (string searchString) =>
            {
                return MovieSearchCache.GetOrAddAsync(searchString, () => ApiClient.SearchByMovie(searchString));
            };

            trendingWeekGetter = () =>
            {
                return MovieSearchCache.GetOrAddAsync("$week", () => ApiClient.GetTrending(true));
            };

            trendingDayGetter = () =>
            {
                return MovieSearchCache.GetOrAddAsync("$day", () => ApiClient.GetTrending(false));
            };

            movieModelDeatilUpdateGetter = (MovieDetailModel movie) =>
            {
                return MovieSearchCache.GetOrAddAsync("$MovieDetailModelUpdate id=" + movie.Id.ToString(), () => ApiClient.UpdateMovieDetail(movie));
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

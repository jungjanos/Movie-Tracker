using Ch9.ApiClient;
using Ch9.Utils;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Ch9.Models;
using System.Net.Http;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Ch9
{
    public partial class App : Application
    {
        public ISettings Settings { get; private set; }
        public MovieGenreSettings MovieGenreSettings { get; private set; }
        public TmdbConfigurationModel TmdbConfiguration { get; private set; }
        public IMovieDetailModelConfigurator MovieDetailModelConfigurator { get; private set; }
        public SearchResultFilter ResultFilter { get; private set; }
        public ITmdbNetworkClient TmdbNetworkClient { get; private set; }
        public ITmdbCachedSearchClient CachedSearchClient { get; private set; }
        public UsersMovieListsService2 UsersMovieListsService2 { get; private set; }
        public IVideoService VideoService { get; private set; }


        public App(HttpClient httpClient = null)
        {
            Settings = new Settings();
            MovieGenreSettings = new MovieGenreSettings();
            ResultFilter = new SearchResultFilter(Settings, MovieGenreSettings);
            TmdbNetworkClient = new TmdbNetworkClient(Settings, httpClient);
            CachedSearchClient = new TmdbCachedSearchClient(TmdbNetworkClient);
            VideoService = new YtExplodeVideoService(httpClient, Settings, CachedSearchClient);

            InitializeComponent();
            MainPage = new LoadingPage();
        }

        protected override async void OnStart()
        {
            var tmdbConfigurationCache = new TmdbConfigurationCache(CachedSearchClient, Properties, this);
            TmdbConfiguration = await tmdbConfigurationCache.FetchTmdbConfiguration();

            MovieDetailModelConfigurator = new MovieDetailModelConfigurator(TmdbConfiguration, MovieGenreSettings);
            UsersMovieListsService2 = new UsersMovieListsService2(Settings, CachedSearchClient, MovieDetailModelConfigurator);
            MainPage = new NavigationPage(new MainTabbedPage());
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

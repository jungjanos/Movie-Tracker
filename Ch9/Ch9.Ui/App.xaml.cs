using Ch9.Models;
using Ch9.Data.LocalSettings;
using Ch9.Services;
using Ch9.Services.Contracts;
using Ch9.Services.ApiCommunicationService;
using Ch9.Services.LocalSettings;
using Ch9.Services.UiModelConfigurationServices;
using Ch9.Services.VideoService;
using Ch9.Ui;
using Ch9.Views;

using System.Net.Http;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Ch9
{
    public partial class App : Application
    {
        private readonly XamarinLocalSettingsPersister _localSettingsPersister;
        private readonly ITmdbConfigurationCache _tmdbConfigurationCache;
        public ISettings Settings { get; private set; }
        public MovieGenreSettingsModel MovieGenreSettings { get; private set; }
        public TmdbConfigurationModel TmdbConfiguration { get; private set; }

        public IMovieDetailModelConfigurator MovieDetailModelConfigurator { get; private set; }
        public IPersonDetailModelConfigurator PersonDetailModelConfigurator { get; private set; }
        public ISearchResultFilter ResultFilter { get; private set; }
        public UsersMovieListsService2 UsersMovieListsService2 { get; private set; }
        public IVideoService VideoService { get; private set; }
        public IWeblinkComposer WeblinkComposer { get; private set; }
        public ITmdbApiService TmdbApiService { get; private set; }
        public IMovieGenreSettingsService MovieGenreSettingsService { get; private set; }

        public App(HttpClient httpClient = null)
        {
            DependencyResolver.ConfigureServices(httpClient, Application.Current.Properties);
            
            _localSettingsPersister = new XamarinLocalSettingsPersister();
            Settings = new Settings(Application.Current.Properties, _localSettingsPersister);
            TmdbApiService = new TmdbApiService(new Data.ApiClient.TmdbCachedSearchClient(new Data.ApiClient.TmdbNetworkClient(httpClient, Settings.ApiKey)));
            TmdbApiService.SessionId = Settings.SessionId;            
            
            _tmdbConfigurationCache = new TmdbConfigurationCache(TmdbApiService, Settings, _localSettingsPersister);

            
            MovieGenreSettingsService = new MovieGenreSettingsService(TmdbApiService, Application.Current.Properties, _localSettingsPersister);
            MovieGenreSettings = MovieGenreSettingsService.GetGenreSetting();
            ResultFilter = new SearchResultFilter(Settings, MovieGenreSettings);


#if GOOGLEPLAY
    VideoService = new VanillaYtVideoService(Settings, TmdbApiService, new VideoPlayerService());
#else
            VideoService = new YtExplodeVideoService(httpClient, Settings, TmdbApiService, new VideoPlayerService());
#endif

            WeblinkComposer = new WeblinkComposer(Settings);
            
            MovieDetailModelConfigurator = new MovieDetailModelConfigurator(Settings, _tmdbConfigurationCache, MovieGenreSettings);
            PersonDetailModelConfigurator = new PersonDetailModelConfigurator(Settings, _tmdbConfigurationCache);
            UsersMovieListsService2 = new UsersMovieListsService2(Settings, TmdbApiService, MovieDetailModelConfigurator);

            InitializeComponent();
            MainPage = new LoadingPage();
        }




        protected override async void OnStart()
        {
            await _tmdbConfigurationCache.FetchAndPersistTmdbConfiguration();            

            if (!Settings.IsLoginPageDeactivationRequested)
            {
                var loginPage = new LoginPage();
                MainPage = new NavigationPage(loginPage);
                MainPage.Navigation.InsertPageBefore(new MainTabbedPage(), loginPage);
            }
            else
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

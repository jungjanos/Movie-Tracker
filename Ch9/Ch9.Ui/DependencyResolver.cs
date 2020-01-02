using Autofac;
using Ch9.Models;
using Ch9.Data.ApiClient;
using Ch9.Data.Contracts;
using Ch9.Data.LocalSettings;
using Ch9.Services.ApiCommunicationService;
using Ch9.Services;
using Ch9.Services.Contracts;
using Ch9.Services.LocalSettings;

using System.Collections.Generic;
using System.Net.Http;
using Ch9.Services.VideoService;

namespace Ch9.Ui
{
    internal static class DependencyResolver
    {
        public static IContainer Container { get; private set; }

        //public static void ConfigureServices(HttpClient httpClient, IDictionary<string, object> settingsKvps)
        //{
        //    var builder = new ContainerBuilder();

        ////builder.RegisterType<XamarinLocalSettingsPersister>().As<IPersistLocalSettings>().SingleInstance();
        //builder.RegisterInstance<IPersistLocalSettings>(new XamarinLocalSettingsPersister());
        //builder.RegisterInstance<ISettings>(new Settings(settingsKvps, Container.Resolve<IPersistLocalSettings>()));

        //var apiService = new TmdbApiService(new TmdbCachedSearchClient(new TmdbNetworkClient(httpClient, Container.Resolve<ISettings>().ApiKey)));
        //apiService.SessionId = Container.Resolve<ISettings>().SessionId;
        //builder.RegisterInstance<ITmdbApiService>(apiService);

        //builder.RegisterInstance<IMovieGenreSettingsService>(new MovieGenreSettingsService(Container.Resolve<ITmdbApiService>(), settingsKvps, Container.Resolve<IPersistLocalSettings>()));

        //builder.RegisterInstance<MovieGenreSettingsModel>(Container.Resolve<IMovieGenreSettingsService>().GetGenreSetting());
        //builder.RegisterInstance<ISearchResultFilter>(new SearchResultFilter(Container.Resolve<ISettings>(), Container.Resolve<MovieGenreSettingsModel>()));

        //builder.RegisterInstance<IVideoService>(
        //    #if GOOGLEPLAY
        //        new VanillaYtVideoService(Container.Resolve<ISettings>(), Container.Resolve<ITmdbApiService>(), new VideoPlayerService())
        //    #else
        //        new YtExplodeVideoService(httpClient, Container.Resolve<ISettings>(), Container.Resolve<ITmdbApiService>(), new VideoPlayerService())
        //    #endif
        //);

        //builder.RegisterInstance<IWeblinkComposer>(new WeblinkComposer(Container.Resolve<ISettings>()));
        //}

        public static void ConfigureServices(HttpClient httpClient, IDictionary<string, object> settingsKvps)
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<XamarinLocalSettingsPersister>().As<IPersistLocalSettings>().SingleInstance();
            builder.RegisterInstance<IDictionary<string, object>>(settingsKvps).ExternallyOwned();

            builder.RegisterType<Settings>().As<ISettings>().SingleInstance();

            builder.Register<ITmdbNetworkClient>(c => new TmdbNetworkClient(httpClient, c.Resolve<ISettings>().ApiKey)).SingleInstance();
            builder.RegisterType<TmdbCachedSearchClient>().As<ITmdbCachedSearchClient>().SingleInstance();

            //builder.RegisterType<TmdbApiService>().As<ITmdbApiService>().SingleInstance();
            builder.Register<ITmdbApiService>(c =>
            {
                var apiService = new TmdbApiService(c.Resolve<ITmdbCachedSearchClient>());
                apiService.SessionId = c.Resolve<ISettings>().SessionId;

                return apiService;
            }).SingleInstance();

            builder.RegisterType<MovieGenreSettingsService>().As<IMovieGenreSettingsService>().SingleInstance();
            builder.Register<MovieGenreSettingsModel>(c => c.Resolve<IMovieGenreSettingsService>().GetGenreSetting()).SingleInstance();

            builder.RegisterType<SearchResultFilter>().As<ISearchResultFilter>().SingleInstance();

            builder.RegisterType<VideoPlayerService>().As<IPlayVideo>().SingleInstance();

            builder.RegisterType<WeblinkComposer>().As<IWeblinkComposer>().SingleInstance();

#if GOOGLEPLAY
            builder.RegisterType<VanillaYtVideoService>().As<IVideoService>().SingleInstance();            
#else
            builder.RegisterType<YtExplodeVideoService>().As<IVideoService>().SingleInstance();
#endif

            Container = builder.Build();
        }
    }
}

/*
 
            _localSettingsPersister = new XamarinLocalSettingsPersister();
            Settings = new Settings(Application.Current.Properties, _localSettingsPersister);
            TmdbApiService = new TmdbApiService(new Data.ApiClient.TmdbCachedSearchClient(new Data.ApiClient.TmdbNetworkClient(httpClient, Settings.ApiKey)));
            _tmdbConfigurationCache = new TmdbConfigurationCache(TmdbApiService, Settings, _localSettingsPersister);

            TmdbApiService.SessionId = Settings.SessionId;            
            
            MovieGenreSettingsService = new MovieGenreSettingsService(TmdbApiService, Application.Current.Properties, _localSettingsPersister);
            MovieGenreSettings = MovieGenreSettingsService.GetGenreSetting();
            ResultFilter = new SearchResultFilter(Settings, MovieGenreSettings);


#if GOOGLEPLAY
    VideoService = new VanillaYtVideoService(Settings, TmdbApiService, new VideoPlayerService());
#else
            VideoService = new YtExplodeVideoService(httpClient, Settings, TmdbApiService, new VideoPlayerService());
#endif

            WeblinkComposer = new WeblinkComposer(Settings);

            InitializeComponent();
            MainPage = new LoadingPage();
        }




        protected override async void OnStart()
        {
            await _tmdbConfigurationCache.FetchAndPersistTmdbConfiguration();
            TmdbConfiguration = _tmdbConfigurationCache.TmdbConfigurationModel;

            MovieDetailModelConfigurator = new MovieDetailModelConfigurator(Settings, TmdbConfiguration, MovieGenreSettings);
            PersonDetailModelConfigurator = new PersonDetailModelConfigurator(Settings, TmdbConfiguration);
            UsersMovieListsService2 = new UsersMovieListsService2(Settings, TmdbApiService, MovieDetailModelConfigurator);

            if (!Settings.IsLoginPageDeactivationRequested)
            {
                var loginPage = new LoginPage();
                MainPage = new NavigationPage(loginPage);
                MainPage.Navigation.InsertPageBefore(new MainTabbedPage(), loginPage);
            }
            else
                MainPage = new NavigationPage(new MainTabbedPage());
     
     
*/

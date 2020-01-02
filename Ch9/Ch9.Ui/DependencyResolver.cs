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
using Ch9.ViewModels;
using Ch9.Services.UiModelConfigurationServices;

namespace Ch9.Ui
{
    internal static class DependencyResolver
    {
        public static IContainer Container { get; private set; }

        public static void ConfigureServices(HttpClient httpClient, IDictionary<string, object> settingsKvps)
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<XamarinLocalSettingsPersister>().As<IPersistLocalSettings>().SingleInstance();
            builder.RegisterInstance<IDictionary<string, object>>(settingsKvps).ExternallyOwned();

            builder.RegisterType<Settings>().As<ISettings>().SingleInstance();

            builder.Register<ITmdbNetworkClient>(c => new TmdbNetworkClient(httpClient, c.Resolve<ISettings>().ApiKey) { SessionId = c.Resolve<ISettings>().SessionId} ).SingleInstance();
            builder.RegisterType<TmdbCachedSearchClient>().As<ITmdbCachedSearchClient>().SingleInstance();
            
            builder.RegisterType<TmdbApiService>().As<ITmdbApiService>().SingleInstance();

            builder.RegisterType<TmdbConfigurationCache>().As<ITmdbConfigurationCache>().SingleInstance();

            builder.RegisterType<MovieGenreSettingsService>().As<IMovieGenreSettingsService>().SingleInstance();
            builder.Register<MovieGenreSettingsModel>(c => c.Resolve<IMovieGenreSettingsService>().GetGenreSetting()).SingleInstance();

            builder.RegisterType<SearchResultFilter>().As<ISearchResultFilter>().SingleInstance();

            builder.RegisterType<VideoPlayerService>().As<IPlayVideo>().SingleInstance();

            builder.RegisterType<WeblinkComposer>().As<IWeblinkComposer>().SingleInstance();

#if GOOGLEPLAY
            builder.RegisterType<VanillaYtVideoService>().As<IVideoService>().SingleInstance();            
#else
            builder.Register<IVideoService>(c => new YtExplodeVideoService(httpClient, c.Resolve<ISettings>(), c.Resolve<ITmdbApiService>(), c.Resolve<IPlayVideo>())).SingleInstance();
#endif
            builder.RegisterType<MovieDetailModelConfigurator>().As<IMovieDetailModelConfigurator>().SingleInstance();
            builder.RegisterType<PersonDetailModelConfigurator>().As<IPersonDetailModelConfigurator>().SingleInstance();
            builder.RegisterType<UsersMovieListsService2>().AsSelf().SingleInstance();


            builder.RegisterType<MainPage3ViewModel>().AsSelf();
            
            
            Container = builder.Build();
        }
    }

    //var a = DependencyResolver.Container.Resolve<IPersistLocalSettings>();
    //var b = DependencyResolver.Container.Resolve<IDictionary<string, object>>();
    //var c = DependencyResolver.Container.Resolve<ISettings>();
    //var d = DependencyResolver.Container.Resolve<ITmdbNetworkClient>();
    //var e = DependencyResolver.Container.Resolve<ITmdbCachedSearchClient>();
    //var f = DependencyResolver.Container.Resolve<ITmdbApiService>();
    //var g = DependencyResolver.Container.Resolve<IMovieGenreSettingsService>();
    //var h = DependencyResolver.Container.Resolve<MovieGenreSettingsModel>();
    //var i = DependencyResolver.Container.Resolve<ISearchResultFilter>();
    //var j = DependencyResolver.Container.Resolve<IPlayVideo>();
    //var k = DependencyResolver.Container.Resolve<IWeblinkComposer>();
    //var l = DependencyResolver.Container.Resolve<IVideoService>();
    //var m = DependencyResolver.Container.Resolve<IMovieDetailModelConfigurator>();
    //var n = DependencyResolver.Container.Resolve<IPersonDetailModelConfigurator>();
    //var o = DependencyResolver.Container.Resolve<UsersMovieListsService2>();
}

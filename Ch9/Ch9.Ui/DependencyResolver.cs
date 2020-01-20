using Ch9.Models;
using Ch9.Data.Contracts;
using Ch9.Data.ApiClient;
using Ch9.Data.LocalSettings;
using Ch9.Services.Contracts;
using Ch9.Services.ApiCommunicationService;
using Ch9.Services;
using Ch9.Services.LocalSettings;
using Ch9.Services.VideoService;
using Ch9.Services.UiModelConfigurationServices;
using Ch9.ViewModels;

using System.Collections.Generic;
using System.Net.Http;
using Autofac;
using Ch9.Services.ViewModelServices;

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

            builder.Register<ITmdbNetworkClient>(c => new TmdbNetworkClient(httpClient, c.Resolve<ISettings>().ApiKey)).SingleInstance();
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
            builder.RegisterType<SigninService>().As<ISigninService>();

            builder.RegisterType<MainPage3ViewModel>().AsSelf();
            builder.RegisterType<TrendingPage3ViewModel>().AsSelf();
            builder.RegisterType<MainSettingsPage2ViewModel>().AsSelf();
            builder.RegisterType <MovieDetailPageViewModel>().AsSelf();
            builder.RegisterType<RecommendationsPage3ViewModel>().AsSelf();
            builder.RegisterType<ReviewsPageViewModel>().AsSelf();
            builder.RegisterType<PersonsMovieCreditsPageViewModel>().AsSelf();
            builder.RegisterType<MovieGenreSettings2PageViewModel>().AsSelf();
            builder.RegisterType<ListsPageViewModel3>().AsSelf();
            builder.RegisterType<AddListPageViewModel>().AsSelf();
            builder.RegisterType<LoginPageViewModel>().AsSelf();

            Container = builder.Build();
        }
    }
}

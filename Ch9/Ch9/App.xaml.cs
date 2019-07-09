﻿using Ch9.ApiClient;
using Ch9.Utils;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using LazyCache;
using System.Threading.Tasks;
using Ch9.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

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

        public App()
        {
            Settings = new Settings();
            MovieGenreSettings = new MovieGenreSettings();
            ResultFilter = new SearchResultFilter(Settings, MovieGenreSettings);
            TmdbNetworkClient = new TmdbNetworkClient(Settings);
            CachedSearchClient = new TmdbCachedSearchClient(TmdbNetworkClient);            

            InitializeComponent();
            // TODO : Add content to loading page
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

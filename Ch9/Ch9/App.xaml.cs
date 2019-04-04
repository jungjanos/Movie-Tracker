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
        public TmdbCachedSearchClient CachedSearchClient { get; private set; }
        

        public App()
        {            
            Settings = new Settings();
            MovieGenreSettings = new MovieGenreSettings();
            ResultFilter = new SearchResultFilter(Settings, MovieGenreSettings);
            CachedSearchClient = new TmdbCachedSearchClient(new TmdbNetworkClient(Settings));            

            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
        }

        protected override async void OnStart()
        {            
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
            var response = await CachedSearchClient.GetTmdbConfiguration(retries, retryDelay);

            if (response.HttpStatusCode.IsSuccessCode())
            {
                var result = JsonConvert.DeserializeObject<TmdbConfigurationModel>(response.Json);
                return result;
            }
            else throw new TimeoutException($"Could not connect TMDB Server to fetch configuration data at application start, retried {retries}-times");
        }
    }
}

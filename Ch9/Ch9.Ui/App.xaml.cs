using Ch9.Services.Contracts;
using Ch9.Ui;
using Ch9.Views;

using System.Net.Http;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Autofac;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Ch9
{
    public partial class App : Application
    {
        private readonly ITmdbConfigurationCache _tmdbConfigurationCache;
        private readonly ISettings _settings;

        public App(HttpClient httpClient = null)
        {
            DependencyResolver.ConfigureServices(httpClient, Application.Current.Properties);    

            using (var scope = DependencyResolver.Container.BeginLifetimeScope())
            {
                _settings = scope.Resolve<ISettings>();
                _tmdbConfigurationCache = scope.Resolve<ITmdbConfigurationCache>();
            }

            InitializeComponent();
            MainPage = new LoadingPage();
        }

        protected override async void OnStart()
        {
            await _tmdbConfigurationCache.FetchAndPersistTmdbConfiguration();            

            if (!_settings.IsLoginPageDeactivationRequested)
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

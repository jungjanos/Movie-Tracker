using Ch9.Models;
using Ch9.Services;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Ch9.ViewModels
{
    public class MainSettingsPage2ViewModel : ViewModelBase
    {
        private ISettings _settings;
        private readonly MovieGenreSettings _movieGenreSettings;
        private readonly ITmdbNetworkClient _tmdbClient;
        private readonly IPageService _pageService;

        public ISettings Settings
        {
            get => _settings;
            set => SetProperty(ref _settings, value);
        }

        public ICommand SearchLanguageChangedCommand { get; private set; }
        public ICommand LoginTappedCommand { get; private set; }

        public MainSettingsPage2ViewModel(
                ISettings settings,
                MovieGenreSettings movieGenreSettings,
                ITmdbNetworkClient tmdbClient,
                IPageService pageService
            ) : base()
        {
            _settings = settings;
            _movieGenreSettings = movieGenreSettings;
            _tmdbClient = tmdbClient;
            _pageService = pageService;
            SearchLanguageChangedCommand = new Command(async () => await OnSearchLanguageChanged());

            LoginTappedCommand = new Command(async () =>
            {
                var hasAccount = _settings.IsLoggedin;
                await OnLoginTapped(hasAccount);
            });
        }

        public async Task SaveChanges() => await Settings.SavePropertiesAsync();

        private async Task OnLoginTapped(bool hasAccount)
        {
            if (hasAccount)
                await LogoutAndDeleteSession();
            else
                await _pageService.PushLoginPageAsync();
        }

        private async Task LogoutAndDeleteSession()
        {
            var sessionIdToDelete = _settings.SessionId;

            if (!string.IsNullOrEmpty(sessionIdToDelete))
                await _tmdbClient.DeleteSession(sessionIdToDelete);

            _settings.SessionId = null;
            _settings.AccountName = null;
            _settings.Password = null;
            _settings.IsLoginPageDeactivationRequested = false;
            await _settings.SavePropertiesAsync();
        }

        private async Task OnSearchLanguageChanged() =>
            await _movieGenreSettings.OnSearchLanguageChanged(Settings.SearchLanguage);
    }
}

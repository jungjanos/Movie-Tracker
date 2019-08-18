using Ch9.Models;
using Ch9.Services;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Ch9.ViewModels
{
    public class MainSettingsPage2ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

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
                PageService pageService
            )
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

        private void OnPropertyChanged([CallerMemberName]string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}

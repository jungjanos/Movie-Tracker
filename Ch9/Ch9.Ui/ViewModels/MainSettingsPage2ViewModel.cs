using Ch9.Services;
using Ch9.Services.Contracts;
using Ch9.Models;

using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Ch9.ViewModels
{
    public class MainSettingsPage2ViewModel : ViewModelBase
    {
        private ISettings _settings;
        private readonly MovieGenreSettingsModel _movieGenreSettings;
        private readonly IMovieGenreSettingsService _movieGenreSettingsService;        
        private readonly ISigninService _signinService;

        public ISettings Settings
        {
            get => _settings;
            set => SetProperty(ref _settings, value);
        }

        public ICommand SearchLanguageChangedCommand { get; private set; }
        public ICommand LoginTappedCommand { get; private set; }
        public ICommand OpenMovieGenreSelectionCommand { get; private set; }

        public MainSettingsPage2ViewModel(
                ISettings settings,
                MovieGenreSettingsModel movieGenreSettings,
                IMovieGenreSettingsService movieGenreSettingsService,                
                ISigninService signinService,
                IPageService pageService
            ) : base(pageService)
        {
            _settings = settings;
            _movieGenreSettings = movieGenreSettings;
            _movieGenreSettingsService = movieGenreSettingsService;            
            _signinService = signinService;
            SearchLanguageChangedCommand = new Command(async () => await OnSearchLanguageChanged());

            LoginTappedCommand = new Command(async () =>
            {
                var hasAccount = _settings.IsLoggedin;
                await OnLoginTapped(hasAccount);
            });

            OpenMovieGenreSelectionCommand = new Command(async () => await _pageService.OpenMovieGenreSelection());
        }

        public async Task SaveChanges() => await Settings.SavePropertiesAsync();

        private async Task OnLoginTapped(bool hasAccount)
        {
            if (hasAccount)                
                await _signinService.LogoutAndDeleteSession();
            else
                await _pageService.PushLoginPageAsync();
        }

        private async Task OnSearchLanguageChanged() =>
            await _movieGenreSettingsService.UpdateGenreListLanguage(Settings.SearchLanguage, _movieGenreSettings); 
    }
}

using Ch9.ApiClient;
using Ch9.Authentication;
using Ch9.Models;
using Ch9.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Ch9
{
    public class MainSettingsPage2ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ISettings _settings;
        private readonly MovieGenreSettings _movieGenreSettings;
        private readonly ITmdbNetworkClient _tmdbClient;
        private readonly IPageService _pageService;

        private string initialAccountName;
        private string initialPassword;

        public ISettings Settings
        {
            get => _settings;
            set => SetProperty(ref _settings, value);
        }

        public ICommand SearchLanguageChangedCommand { get; private set; }            

        public MainSettingsPage2ViewModel(ISettings settings,
            MovieGenreSettings movieGenreSettings,
            ITmdbNetworkClient tmdbClient,
            PageService pageService)
        {
            _settings = settings;
            _movieGenreSettings = movieGenreSettings;
            _tmdbClient = tmdbClient;
            _pageService = pageService;
            SearchLanguageChangedCommand = new Command(async () => await OnSearchLanguageChanged());
        }

        // this method should be called when AccountName/Password has changed
        // should try to generate a new SessionId for the account
        // should try to dispose any previous SessionId if available (best effort)
        // Returns error message is failed
        private async Task<string> TryTmdbSignin()
        {
            if (!string.IsNullOrEmpty(Settings.SessionId))
                await _tmdbClient.DeleteSession(Settings.SessionId);


            var createRequestTokenResult = await _tmdbClient.CreateRequestToken(3, 1000);

            try
            {
                var token = JsonConvert.DeserializeObject<RequestToken>(createRequestTokenResult.Json);

                if (!token.Success)
                    return "Error getting request token from TMDB server";

                var validateTokenResult =
                    await _tmdbClient.ValidateRequestTokenWithLogin(Settings.AccountName, Settings.Password, token.Token, 3, 1000);

                if (!validateTokenResult.HttpStatusCode.IsSuccessCode())
                    return "Error validating request token";

                string validatedToken = JsonConvert.DeserializeObject<RequestToken>(validateTokenResult.Json).Token;

                var sessionIdResult = await _tmdbClient.CreateSessionId(validatedToken, 3, 1000);

                if (!sessionIdResult.HttpStatusCode.IsSuccessCode())
                    return "Server failed to return a valid Session Id";

                var session = JsonConvert.DeserializeObject<SessionIdResponseModel>(sessionIdResult.Json);

                Settings.SessionId = session.SessionId;
            }
            catch (Exception ex)
            {
                return $"Failure at authenticating user, message: {ex.Message}";
            }
            return string.Empty;
        }

        private async Task OnSearchLanguageChanged()
        {           
            await _movieGenreSettings.OnSearchLanguageChanged(Settings.SearchLanguage);
        }

        public void InitializeOnAppearing()
        {
            initialAccountName = Settings.AccountName;
            initialPassword = Settings.Password;
        }

        public async Task ValidateOnDisappearing()
        {
            await ValidateSettings();
            await Application.Current.SavePropertiesAsync();

            if (initialAccountName != Settings.AccountName || initialPassword != Settings.Password)
            {
                string result = await TryTmdbSignin();

                if (string.IsNullOrEmpty(result))
                {
                    //   await App.Current.MainPage.DisplayAlert("Success", "Logon successfull", "Ok");
                }
                //else
                //    await App.Current.MainPage.DisplayAlert("Logon error", $"Message: {result}", "Ok");
            }            
        }

        private async Task<bool> ValidateSettings()
        {
            return await ValidateAccountSettings();
        }

        private async Task<bool> ValidateAccountSettings()
        {
            // Validation fails, if the HasAccount switch is set but the username or password are empty
            bool validationResult =
            Settings.HasTmdbAccount ? !(string.IsNullOrWhiteSpace(Settings.AccountName) || string.IsNullOrWhiteSpace(Settings.Password)) : true;

            if (!validationResult)
            {
                Settings.HasTmdbAccount = false;
                await _pageService.DisplayAlert("Error", "Account name and password must be set if account options are selected!", "Ok");
            }
            return validationResult;
        }


        private void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
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

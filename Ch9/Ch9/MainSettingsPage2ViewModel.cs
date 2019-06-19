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

        public ISettings Settings
        {
            get => _settings;
            set => SetProperty(ref _settings, value);
        }

        private bool _notWaitingOnServer;
        // This property indicates whether any server based task is still running
        // governs IsActive and CanExecute properties of controls/commands  
        public bool NotWaitingOnServer
        {
            get => _notWaitingOnServer;
            set
            {
                if (SetProperty(ref _notWaitingOnServer, value))
                    OnPropertyChanged(nameof(UserProvidedAccountAndPasswordNotEmptyAndNotWaiting));
            }
        }

        private bool _validateAccountOnServerSwitch;
        public bool ValidateAccountOnServerSwitch
        {
            get => _validateAccountOnServerSwitch;
            set
            {
                if (value == false)
                {
                    // true -> false state change
                    if (SetProperty(ref _validateAccountOnServerSwitch, value))
                    {
                        UserProvidedAccountName = null;
                        UserProvidedPassword = null;

                        Settings.AccountName = null;
                        Settings.Password = null;

                        string sessionIdToDelete = Settings.SessionId;
                        DeleteSessionIdCommand.Execute(sessionIdToDelete);
                        Settings.SessionId = null;

                        Settings.HasTmdbAccount = false;                        
                        OnPropertyChanged(nameof(Settings));
                    }
                }
                else
                {
                    // false -> true state change
                    if (SetProperty(ref _validateAccountOnServerSwitch, value))
                    {
                        (ValidateAccountOnServerCommand as Command).Execute(null);
                    }
                }
            }
        }

        private string _userProvidedAccountName;
        public string UserProvidedAccountName
        {
            get => _userProvidedAccountName;
            set
            {
                if (SetProperty(ref _userProvidedAccountName, value))
                    OnPropertyChanged(nameof(UserProvidedAccountAndPasswordNotEmptyAndNotWaiting));
            }
        }

        private string _userProvidedPassword;       
        public string UserProvidedPassword
        {
            get => _userProvidedPassword;
            set
            {
                if (SetProperty(ref _userProvidedPassword, value))
                    OnPropertyChanged(nameof(UserProvidedAccountAndPasswordNotEmptyAndNotWaiting));
            }
        }

        public bool UserProvidedAccountAndPasswordNotEmptyAndNotWaiting
        {
            get => !string.IsNullOrWhiteSpace(UserProvidedAccountName) && !string.IsNullOrWhiteSpace(UserProvidedPassword) && NotWaitingOnServer;
        }

        private ICommand ValidateAccountOnServerCommand { get; set; }
        public Command<string> DeleteSessionIdCommand { get; private set; }        
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
            ValidateAccountOnServerCommand = new Command(async () => await OnValidateAccountOnServer());
            DeleteSessionIdCommand = new Command<string>(async sessionId => await OnDeleteSessionId(sessionId));            

            if (ValidateAccountOnServerSwitch = Settings.HasTmdbAccount)
            {
                UserProvidedAccountName = Settings.AccountName;
                UserProvidedPassword = Settings.Password;
            }
            NotWaitingOnServer = true;            
        }

        public async Task SaveChanges() => await Settings.SavePropertiesAsync();

        private async Task OnDeleteSessionId(string sessionIdToDelete)
        {                      
            if (!string.IsNullOrEmpty(sessionIdToDelete))
                await _tmdbClient.DeleteSession(sessionIdToDelete);
        }

        private async Task OnValidateAccountOnServer()
        {
            NotWaitingOnServer = false;
            var result = await TryTmdbSignin();
            if (result.Success)
            {
                Settings.HasTmdbAccount = true;
                Settings.SessionId = result.NewSessionId;

                Settings.AccountName = UserProvidedAccountName;
                Settings.Password = UserProvidedPassword;                
            }
            else
            {
                Settings.SessionId = null;
                ValidateAccountOnServerSwitch = false;
            }
            OnPropertyChanged(nameof(Settings));            
            NotWaitingOnServer = true;
        }

        // this method should be called when AccountName/Password has changed
        // should try to generate a new SessionId for the account
        // should try to dispose any previous SessionId if available (best effort)
        // Returns bool: success status and the new string: Sessionid
        //
        // CALLER IS RESPONSIBLE TO SET Settings.SessionId according to the result.
        // results CAN NOT be ignored (side effect on server)
        private async Task<(bool Success, string NewSessionId)> TryTmdbSignin()
        {
            string nullStr = null;
            var result = (Success: false, NewSessionId: nullStr);

            SessionIdResponseModel newSession = null;

            string sessionIdToDelete = Settings.SessionId;

            if (!string.IsNullOrEmpty(sessionIdToDelete))
                await _tmdbClient.DeleteSession(sessionIdToDelete);

            try
            {
                var createRequestTokenResult = await _tmdbClient.CreateRequestToken(3, 1000);
                if (!createRequestTokenResult.HttpStatusCode.IsSuccessCode())
                {
                    await _pageService.DisplayAlert("Sign in error", $"Error getting request token from TMDB server, server response: {createRequestTokenResult.HttpStatusCode}", "Ok");
                    return result;
                }

                var token = JsonConvert.DeserializeObject<RequestToken>(createRequestTokenResult.Json);
                if (!token.Success)
                {
                    await _pageService.DisplayAlert("Sign in error", "TMDB server indicated failure in request token", "Ok");
                    return result;
                }

                var validateTokenResult = await _tmdbClient.ValidateRequestTokenWithLogin(UserProvidedAccountName, UserProvidedPassword, token.Token, 3, 1000);

                if (!validateTokenResult.HttpStatusCode.IsSuccessCode())
                {
                    await _pageService.DisplayAlert("Sign in error", $"TMDB server reported error when authenticating with supplied account credentials, server response: {validateTokenResult.HttpStatusCode}", "Ok");
                    return result;
                }

                string validatedToken = JsonConvert.DeserializeObject<RequestToken>(validateTokenResult.Json).Token;

                var sessionIdResult = await _tmdbClient.CreateSessionId(validatedToken, 3, 1000);

                if (!sessionIdResult.HttpStatusCode.IsSuccessCode())
                {
                    await _pageService.DisplayAlert("Sign in error", $"TMDB server reported error when creating a new session id, server response: {sessionIdResult.HttpStatusCode}", "Ok");
                    return result;
                }

                newSession = JsonConvert.DeserializeObject<SessionIdResponseModel>(sessionIdResult.Json);
            }
            catch (Exception ex)
            {
                await _pageService.DisplayAlert("Sign in error", $"Exception was thrown during sign in procedure. Details: {ex.Message}", "Ok");
                return result;
            }

            result.Success = !string.IsNullOrWhiteSpace(newSession?.SessionId);
            result.NewSessionId = newSession?.SessionId;

            return result;
        }        

        private async Task OnSearchLanguageChanged()
        {
            await _movieGenreSettings.OnSearchLanguageChanged(Settings.SearchLanguage);
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

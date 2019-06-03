using System;
using System.Threading.Tasks;
using Ch9.ApiClient;
using Ch9.Authentication;
using Ch9.Models;
using Ch9.Utils;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ch9
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainSettingsPage : ContentPage
	{
        private readonly ISettings _settings;
        private readonly MovieGenreSettings _movieGenreSettings;
        private readonly ITmdbNetworkClient _tmdbClient;  

        private string initialAccountName;
        private string initialPassword;

		public MainSettingsPage ()
		{
            _settings = ((App)Application.Current).Settings;
            _movieGenreSettings = ((App)Application.Current).MovieGenreSettings;
            _tmdbClient = ((App)Application.Current).TmdbNetworkClient;
            InitializeComponent ();
            BindingContext = _settings;
		}

        private async void OnSearchLanguage_Changed(object sender, EventArgs e)
        {
            _settings.SearchLanguage =  searchLanguagePicker.Items[searchLanguagePicker.SelectedIndex];

            await _movieGenreSettings.OnSearchLanguageChanged(_settings.SearchLanguage);           

        }

        private async void OnSelectGenres_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new GenreSettingsPage());
        }

        protected override void OnAppearing()
        {
            initialAccountName = _settings.AccountName;
            initialPassword = _settings.Password;
        }

        protected override async void OnDisappearing()
        {
            await ValidateSettings();
            await Application.Current.SavePropertiesAsync();

            if (initialAccountName != _settings.AccountName || initialPassword !=_settings.Password)
            {
                string result = await TryTmdbSignin();

                if (string.IsNullOrEmpty(result))
                {
                 //   await App.Current.MainPage.DisplayAlert("Success", "Logon successfull", "Ok");
                }
                else
                    await App.Current.MainPage.DisplayAlert("Logon error", $"Message: {result}", "Ok");
            }
            base.OnDisappearing();
        }        

        private async Task<bool> ValidateSettings()
        {
            return await ValidateAccountSettings();
        }

        private async Task<bool> ValidateAccountSettings()
        {
            // Validation fails, if the HasAccount switch is set but the username or password are empty
            bool validationResult =
            _settings.HasTmdbAccount ? !(string.IsNullOrWhiteSpace(_settings.AccountName) || string.IsNullOrWhiteSpace(_settings.Password)) : true;

            if (!validationResult)
            {
                hasAccountSwitch.On = false;
                await DisplayAlert("Error", "Account name and password must be set if account options are selected!", "Ok");                
            }
            return validationResult;
        }

        // this method should be called when AccountName/Password has changed
        // should try to generate a new SessionId for the account
        // should try to dispose any previous SessionId if available (best effort)
        // Returns error message is failed
        private async Task<string> TryTmdbSignin()
        {           
            if (!string.IsNullOrEmpty(_settings.SessionId))
                await _tmdbClient.DeleteSession(_settings.SessionId);


            var createRequestTokenResult = await _tmdbClient.CreateRequestToken(3, 1000);

            try
            {
                var token = JsonConvert.DeserializeObject<RequestToken>(createRequestTokenResult.Json);

                if (!token.Success)
                    return "Error getting request token from TMDB server";

                var validateTokenResult = 
                    await _tmdbClient.ValidateRequestTokenWithLogin(_settings.AccountName, _settings.Password, token.Token, 3, 1000);

                if (!validateTokenResult.HttpStatusCode.IsSuccessCode())
                    return "Error validating request token";

                string validatedToken = JsonConvert.DeserializeObject<RequestToken>(validateTokenResult.Json).Token;

                var sessionIdResult = await _tmdbClient.CreateSessionId(validatedToken, 3, 1000);

                if (!sessionIdResult.HttpStatusCode.IsSuccessCode())
                    return "Server failed to return a valid Session Id";

                var session = JsonConvert.DeserializeObject<SessionIdResponseModel>(sessionIdResult.Json);
                
                _settings.SessionId = session.SessionId;
            }
            catch (Exception ex)
            {
                return $"Failure at authenticating user, message: {ex.Message}";
            }
            return string.Empty;
        }

    }
}
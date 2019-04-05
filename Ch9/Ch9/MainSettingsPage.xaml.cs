using System;
using System.Threading.Tasks;
using Ch9.Models;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ch9
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainSettingsPage : ContentPage
	{
        private Settings settings;
        private MovieGenreSettings movieGenreSettings;

		public MainSettingsPage ()
		{
            settings = ((App)Application.Current).Settings;
            movieGenreSettings = ((App)Application.Current).MovieGenreSettings;
            InitializeComponent ();
            BindingContext = settings;
		}

        private async void OnSearchLanguage_Changed(object sender, EventArgs e)
        {
            settings.SearchLanguage =  searchLanguagePicker.Items[searchLanguagePicker.SelectedIndex];

            await movieGenreSettings.OnSearchLanguageChanged(settings.SearchLanguage);           

        }

        private async void OnSelectGenres_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new GenreSettingsPage());
        }

        protected override async void OnDisappearing()
        {
            await ValidateSettings();
            await Application.Current.SavePropertiesAsync();
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
            settings.HasTmdbAccount ? !(string.IsNullOrWhiteSpace(settings.AccountName) || string.IsNullOrWhiteSpace(settings.Password)) : true;

            if (!validationResult)
            {
                hasAccountSwitch.On = false;
                await DisplayAlert("Error", "Account name and password must be set if account options are selected!", "Ok");                
            }
            return validationResult;
        }

    }
}
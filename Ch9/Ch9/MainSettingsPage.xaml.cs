using System;
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
            await Application.Current.SavePropertiesAsync();
            base.OnDisappearing();
        }

    }
}
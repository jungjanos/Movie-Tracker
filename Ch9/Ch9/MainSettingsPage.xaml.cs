using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

		public MainSettingsPage ()
		{
            settings = ((App)Application.Current).Settings;
            InitializeComponent ();
            BindingContext = settings;
		}

        private void OnSearchLanguage_Changed(object sender, EventArgs e)
        {
            settings.SearchLanguage =  searchLanguagePicker.Items[searchLanguagePicker.SelectedIndex];
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
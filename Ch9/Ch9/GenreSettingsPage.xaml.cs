using Ch9.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ch9
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GenreSettingsPage : ContentPage
	{
        private MovieGenreSettings movieGenreSettings;

		public GenreSettingsPage ()
		{
            movieGenreSettings = ((App)Application.Current).MovieGenreSettings;
			InitializeComponent ();
            //genreListView.BindingContext = movieGenreSettings.GenreSelectionDisplay;
            genreListView.ItemsSource = movieGenreSettings.GenreSelectionDisplay;
        }


        protected override async void OnDisappearing()
        {
            await Application.Current.SavePropertiesAsync();
            base.OnDisappearing();
        }
    }
}
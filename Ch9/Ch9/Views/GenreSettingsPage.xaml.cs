using Ch9.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ch9.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GenreSettingsPage : ContentPage
	{
        private MovieGenreSettings movieGenreSettings;

		public GenreSettingsPage ()
		{
            movieGenreSettings = ((App)Application.Current).MovieGenreSettings;
			InitializeComponent ();            
            genreListView.ItemsSource = movieGenreSettings.GenreSelectionDisplay;
        }

        protected override async void OnDisappearing()
        {
            await Application.Current.SavePropertiesAsync();
            base.OnDisappearing();
        }
    }
}
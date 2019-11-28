using Ch9.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ch9.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GenreSettingsPage2 : ContentPage
    {
        public MovieGenreSettings ViewModel
        {
            get => BindingContext as MovieGenreSettings;
            set => BindingContext = value;
        }

        public GenreSettingsPage2()
        {
            ViewModel = ((App)Application.Current).MovieGenreSettings;
            InitializeComponent();
        }
    }
}
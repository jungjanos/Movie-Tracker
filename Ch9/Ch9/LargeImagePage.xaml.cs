using Ch9.Models;
using Ch9.Utils;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ch9
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LargeImagePage : ContentPage
    {
        public MovieDetailPageViewModel ViewModel
        {
            get => BindingContext as MovieDetailPageViewModel;
            set => BindingContext = value;
        }

        public LargeImagePage(MovieDetailPageViewModel viewModel)
        {
            ViewModel = viewModel;

            InitializeComponent();
        }
    }
}
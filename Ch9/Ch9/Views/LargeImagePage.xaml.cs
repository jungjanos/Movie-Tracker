using Ch9.Services;
using Ch9.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ch9.Views
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
            InitializeComponent();
            ViewModel = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            MessagingCenter.Send(this, MessagingCenterMessages.SET_LANDSCAPE);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Send(this, MessagingCenterMessages.SET_PORTRAIT);
        }
    }
}
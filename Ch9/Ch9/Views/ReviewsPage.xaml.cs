using Ch9.Services;
using Ch9.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ch9.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReviewsPage : ContentPage
    {
        public ReviewsPageViewModel ViewModel
        {
            get => BindingContext as ReviewsPageViewModel;
            set => BindingContext = value;
        }

        public ReviewsPage(MovieDetailPageViewModel parentViewModel)
        {
            ViewModel = new ReviewsPageViewModel(
                parentViewModel,
                ((App)Application.Current).Settings,
                ((App)Application.Current).CachedSearchClient,
                new PageService(this)
                );

            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await ViewModel.Initialize();
        }
    }
}
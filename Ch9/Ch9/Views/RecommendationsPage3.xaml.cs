using Ch9.Models;
using Ch9.Services;
using Ch9.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ch9.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RecommendationsPage3 : ContentPage
    {
        public RecommendationsPage3ViewModel ViewModel
        {
            get => BindingContext as RecommendationsPage3ViewModel;
            set => BindingContext = value;
        }
        public RecommendationsPage3(MovieDetailModel movie)
        {
            InitializeComponent();

            ViewModel = new RecommendationsPage3ViewModel(
                movie,
                ((App)Application.Current).Settings,
                ((App)Application.Current).CachedSearchClient,
                ((App)Application.Current).ResultFilter,
                ((App)Application.Current).MovieDetailModelConfigurator,
                new PageService(this));
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await ViewModel.Initialize();
        }
    }
}
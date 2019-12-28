using Ch9.Services;
using Ch9.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ch9.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TrendingPage2 : ContentPage
    {
        public TrendingPage3ViewModel ViewModel
        {
            get => BindingContext as TrendingPage3ViewModel;
            set => BindingContext = value;
        }
        public TrendingPage2()
        {
            InitializeComponent();

            ViewModel = new TrendingPage3ViewModel(
            ((App)Application.Current).Settings,            
            ((App)Application.Current).TmdbApiService,
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
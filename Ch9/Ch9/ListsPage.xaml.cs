using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ch9
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListsPage : ContentPage
    {
        public ListsPageViewModel ViewModel
        {
            get => BindingContext as ListsPageViewModel;
            set => BindingContext = value;
        }

        public ListsPage()
        {
            ViewModel = new ListsPageViewModel(
                ((App)Application.Current).Settings,
                ((App)Application.Current).CachedSearchClient,
                ((App)Application.Current).MovieDetailModelConfigurator);

            InitializeComponent();
        }

        protected override async void OnAppearing()
        {            
            base.OnAppearing();
            await ViewModel.Initialize();            
        }

        private async void OnRemoveClicked(object sender, System.EventArgs e)
        {
            await ViewModel.RemoveMovieFromList();
        }
    }
}
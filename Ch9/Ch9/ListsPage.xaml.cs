using Ch9.Utils;
using System.Threading.Tasks;
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

        Task vMInitializer; 

        public ListsPage()
        {
            ViewModel = new ListsPageViewModel(
                ((App)Application.Current).Settings,
                ((App)Application.Current).CachedSearchClient,
                ((App)Application.Current).MovieDetailModelConfigurator,
                new PageService(this));

            vMInitializer = ViewModel.Initialize();
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {            
            base.OnAppearing();
            await vMInitializer;  
        }

        private async void OnRemoveClicked(object sender, System.EventArgs e)
        {
            await ViewModel.RemoveMovieFromList();
        }
    }
}
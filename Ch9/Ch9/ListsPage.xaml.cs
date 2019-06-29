using Ch9.Utils;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ch9
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListsPage : ContentPage
    {
        public ListsPageViewModel3 ViewModel
        {
            get => BindingContext as ListsPageViewModel3;
            set => BindingContext = value;
        }

        Task vMInitializer;

        public ListsPage()
        {
            ViewModel = new ListsPageViewModel3(
                    ((App)Application.Current).UsersMovieListsService2,
                    ((App)Application.Current).Settings,                    
                    new PageService(this));


            vMInitializer = ViewModel.Initialize();
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            await vMInitializer;
            base.OnAppearing();
            
        }

        private async void OnRemoveClicked(object sender, System.EventArgs e)
        {
            //await ViewModel.RemoveMovieFromList();
        }
    }
}
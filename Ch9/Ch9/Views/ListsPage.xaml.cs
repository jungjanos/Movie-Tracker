using Ch9.Utils;
using Ch9.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ch9.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListsPage : ContentPage
    {
        readonly Task vMInitializer;

        public ListsPageViewModel3 ViewModel
        {
            get => BindingContext as ListsPageViewModel3;
            set => BindingContext = value;
        }

        public ListsPage()
        {
            ViewModel = new ListsPageViewModel3(
                    ((App)Application.Current).UsersMovieListsService2,                                    
                    new PageService(this));

            vMInitializer = ViewModel.Initialize();
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            await vMInitializer;
            base.OnAppearing();            
        }
    }
}
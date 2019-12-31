using Ch9.Services;
using Ch9.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ch9.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListsPage : ContentPage
    {
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

            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            await ViewModel.Initialize();
            base.OnAppearing();
        }
    }
}
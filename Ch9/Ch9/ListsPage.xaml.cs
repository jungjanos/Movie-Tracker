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
                ((App)Application.Current).CachedSearchClient);

            InitializeComponent();
        }

        protected override async void OnAppearing()
        {            
            base.OnAppearing();
            await ViewModel.Initialize();            
        }

    }
}
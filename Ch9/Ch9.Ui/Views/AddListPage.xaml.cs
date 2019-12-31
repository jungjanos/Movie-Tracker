using Ch9.Services;
using Ch9.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ch9.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddListPage : ContentPage
    {
        public AddListPageViewModel ViewModel
        {
            get => BindingContext as AddListPageViewModel;
            private set => BindingContext = value;
        }

        public AddListPage(ListsPageViewModel3 previousPageViewModel)
        {
            ViewModel = new AddListPageViewModel(previousPageViewModel, new PageService(this));
            InitializeComponent();
        }
    }
}
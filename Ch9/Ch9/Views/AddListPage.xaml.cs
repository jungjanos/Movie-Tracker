using Ch9.Utils;
using Ch9.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ch9.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddListPage : ContentPage
    {
        public AddListPageViewModel ViewModel {
            get => BindingContext as AddListPageViewModel;
            private set => BindingContext = value;
        }        

        public AddListPage(AddListPageViewModel viewModel)
        {
            viewModel.PageService = new PageService(this);
            ViewModel = viewModel;
            InitializeComponent();
        }        
    }
}
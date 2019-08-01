using Ch9.Utils;
using Ch9.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ch9.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage3 : ContentPage
    {
        public MainPage3ViewModel ViewModel
        {
            get => BindingContext as MainPage3ViewModel;
            set => BindingContext = value;
        }
        public MainPage3()
        {
            InitializeComponent();

            ViewModel = new MainPage3ViewModel(
                ((App)Application.Current).Settings,
                ((App)Application.Current).CachedSearchClient,
                ((App)Application.Current).ResultFilter,
                ((App)Application.Current).MovieDetailModelConfigurator,
                new PageService(this));
        }          
    }
}
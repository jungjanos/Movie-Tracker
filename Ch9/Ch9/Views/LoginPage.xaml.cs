using Ch9.Services;
using Ch9.ViewModels;
using Xamarin.Forms;

namespace Ch9.Views
{
    //[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPageViewModel ViewModel
        {
            get => BindingContext as LoginPageViewModel;
            set => BindingContext = value;
        }

        public LoginPage(string userName = null, string password = null)
        {
            InitializeComponent();

            ViewModel = new LoginPageViewModel(
                ((App)Application.Current).Settings,
                ((App)Application.Current).TmdbApiService,
                new PageService(this),
                userName,
                password
            );

        }
    }
}
using Ch9.Services;
using Ch9.Ui;
using Ch9.ViewModels;

using Xamarin.Forms;
using Autofac;

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

            using (var scope = DependencyResolver.Container.BeginLifetimeScope())
            {
                ViewModel = scope.Resolve<LoginPageViewModel>(                    
                        new TypedParameter(typeof(IPageService), new PageService(this)),
                        new NamedParameter("username", userName),
                        new NamedParameter("password", password)
                    );
            }
        }
    }
}
using Ch9.Services;
using Ch9.Ui;
using Ch9.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Autofac;

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

            using (var scope = DependencyResolver.Container.BeginLifetimeScope())
            {
                ViewModel = scope.Resolve<MainPage3ViewModel>(new TypedParameter(typeof(IPageService), new PageService(this)));
            }
        }
    }
}
using Ch9.Services;
using Ch9.Ui;
using Ch9.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Autofac;

namespace Ch9.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TrendingPage2 : ContentPage
    {
        public TrendingPage3ViewModel ViewModel
        {
            get => BindingContext as TrendingPage3ViewModel;
            set => BindingContext = value;
        }
        public TrendingPage2()
        {
            InitializeComponent();

            using(var scope = DependencyResolver.Container.BeginLifetimeScope())
            {
                ViewModel = scope.Resolve<TrendingPage3ViewModel>(new TypedParameter[] { new TypedParameter(typeof(IPageService), new PageService(this)) });
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await ViewModel.Initialize();
        }
    }
}
using Ch9.Services;
using Ch9.Ui;
using Ch9.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Autofac;

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

            using (var scope = DependencyResolver.Container.BeginLifetimeScope())
            {
                ViewModel = scope.Resolve<ListsPageViewModel3>(
                    new TypedParameter[] {                        
                        new TypedParameter(typeof(IPageService), new PageService(this))
                    });
            }

            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            await ViewModel.Initialize();
            base.OnAppearing();
        }
    }
}
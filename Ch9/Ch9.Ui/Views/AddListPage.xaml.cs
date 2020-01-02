using Ch9.Services;
using Ch9.Ui;
using Ch9.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Autofac;

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
            using (var scope = DependencyResolver.Container.BeginLifetimeScope())
            {
                ViewModel = scope.Resolve<AddListPageViewModel>(
                    new TypedParameter[] {
                        new TypedParameter(typeof(ListsPageViewModel3), previousPageViewModel),
                        new TypedParameter(typeof(IPageService), new PageService(this))
                    });
            }

            InitializeComponent();
        }
    }
}
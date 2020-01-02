using Ch9.Services;
using Ch9.Ui;
using Ch9.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Autofac;

namespace Ch9.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReviewsPage : ContentPage
    {
        public ReviewsPageViewModel ViewModel
        {
            get => BindingContext as ReviewsPageViewModel;
            set => BindingContext = value;
        }

        public ReviewsPage(MovieDetailPageViewModel parentViewModel)
        {
            using (var scope = DependencyResolver.Container.BeginLifetimeScope())
            {
                ViewModel = scope.Resolve<ReviewsPageViewModel>(
                    new TypedParameter[] {
                        new TypedParameter(typeof(MovieDetailPageViewModel), parentViewModel),
                        new TypedParameter(typeof(IPageService), new PageService(this))
                    });
            }

            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await ViewModel.Initialize();
        }
    }
}
using Ch9.Models;
using Ch9.Services;
using Ch9.Ui;
using Ch9.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Autofac;

namespace Ch9.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RecommendationsPage3 : ContentPage
    {
        public RecommendationsPage3ViewModel ViewModel
        {
            get => BindingContext as RecommendationsPage3ViewModel;
            set => BindingContext = value;
        }
        public RecommendationsPage3(MovieDetailModel movie)
        {
            InitializeComponent();

            using (var scope = DependencyResolver.Container.BeginLifetimeScope())
            {
                ViewModel = scope.Resolve<RecommendationsPage3ViewModel>(
                    new TypedParameter[] {
                        new TypedParameter(typeof(MovieDetailModel), movie),
                        new TypedParameter(typeof(IPageService), new PageService(this))
                    });
            }
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await ViewModel.Initialize();
        }
    }
}
using Ch9.Services;
using Ch9.Models;
using Ch9.ViewModels;
using Ch9.Ui;

using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Autofac;

namespace Ch9.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MovieDetailPage : ContentPage
    {
        public MovieDetailPageViewModel ViewModel
        {
            get => BindingContext as MovieDetailPageViewModel;
            set => BindingContext = value;
        }

        public MovieDetailPage(MovieDetailModel movie)
        {
            using (var scope = DependencyResolver.Container.BeginLifetimeScope())
            {
                ViewModel = scope.Resolve<MovieDetailPageViewModel>( 
                        new TypedParameter(typeof(MovieDetailModel), movie),
                        new TypedParameter(typeof(IPageService), new PageService(this))
                    );
            }

            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            await ViewModel.Initialize();
            base.OnAppearing();
        }

        private void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection?.FirstOrDefault() != null)
            {
                var selectedPerson = e.CurrentSelection?.FirstOrDefault();
                movieCastList.SelectedItem = null;
                ViewModel.MovieCastPersonTappedCommand.Execute(selectedPerson);
            }
        }
    }
}
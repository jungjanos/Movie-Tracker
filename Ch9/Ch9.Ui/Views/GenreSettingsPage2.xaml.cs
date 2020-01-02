using Ch9.Services;
using Ch9.Ui;
using Ch9.ViewModels;

using Autofac;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ch9.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GenreSettingsPage2 : ContentPage
    {
        public MovieGenreSettings2PageViewModel ViewModel
        {
            get => BindingContext as MovieGenreSettings2PageViewModel;
            set => BindingContext = value;
        }

        public GenreSettingsPage2()
        {
            using (var scope = DependencyResolver.Container.BeginLifetimeScope())
            {
                ViewModel = scope.Resolve<MovieGenreSettings2PageViewModel>(
                    new TypedParameter[] {                        
                        new TypedParameter(typeof(IPageService), new PageService(this))
                    });
            }
            InitializeComponent();
        }

        protected override async void OnDisappearing()
        {
            await ViewModel.PersistMovieGenreSettings();
            base.OnDisappearing();
        }
    }
}
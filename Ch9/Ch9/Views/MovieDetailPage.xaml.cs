using Ch9.Models;
using Ch9.Utils;
using Ch9.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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

        readonly Task vMInitializer;

        public MovieDetailPage(MovieDetailModel movie)
        {
            ViewModel = new MovieDetailPageViewModel(
                movie,
                ((App)Application.Current).Settings,
                ((App)Application.Current).CachedSearchClient,
                ((App)Application.Current).UsersMovieListsService2,
                ((App)Application.Current).MovieDetailModelConfigurator,
                ((App)Application.Current).PersonDetailModelConfigurator,
                ((App)Application.Current).VideoService,
                new PageService(this)
                );
            vMInitializer = ViewModel.Initialize();

            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await vMInitializer;
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
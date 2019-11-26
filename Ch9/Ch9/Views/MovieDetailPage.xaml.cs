using Ch9.Models;
using Ch9.Services;
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
                ((App)Application.Current).WeblinkComposer,
                new PageService(this)
                );            

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
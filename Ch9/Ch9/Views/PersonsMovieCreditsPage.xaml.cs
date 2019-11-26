using Ch9.Ui.Contracts.Models;
using Ch9.Services;
using Ch9.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ch9.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PersonsMovieCreditsPage : ContentPage
    {
        public PersonsMovieCreditsPageViewModel ViewModel
        {
            get => BindingContext as PersonsMovieCreditsPageViewModel;
            set => BindingContext = value;
        }

        public PersonsMovieCreditsPage(PersonsDetailsModel personDetails)
        {
            ViewModel = new PersonsMovieCreditsPageViewModel(
                personDetails,
                ((App)Application.Current).Settings,                
                ((App)Application.Current).TmdbApiService,
                ((App)Application.Current).MovieDetailModelConfigurator,
                ((App)Application.Current).PersonDetailModelConfigurator,
                ((App)Application.Current).WeblinkComposer,
                new PageService(this)
                );

            InitializeComponent();
        }

    }
}
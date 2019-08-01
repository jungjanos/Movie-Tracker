using Ch9.Models;
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

        public PersonsMovieCreditsPage(GetPersonsDetailsModel personDetails, GetPersonsMovieCreditsModel personsMovieCredits)
        {
            ViewModel = new PersonsMovieCreditsPageViewModel(
                personDetails,
                personsMovieCredits,
                ((App)Application.Current).Settings,                               
                ((App)Application.Current).MovieDetailModelConfigurator,
                ((App)Application.Current).PersonDetailModelConfigurator,
                new PageService(this)
                );

            InitializeComponent();
        }
    }
}
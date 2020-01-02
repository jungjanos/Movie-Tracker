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
    public partial class PersonsMovieCreditsPage : ContentPage
    {
        public PersonsMovieCreditsPageViewModel ViewModel
        {
            get => BindingContext as PersonsMovieCreditsPageViewModel;
            set => BindingContext = value;
        }

        public PersonsMovieCreditsPage(PersonsDetailsModel personDetails)
        {
            using (var scope = DependencyResolver.Container.BeginLifetimeScope())
            {
                ViewModel = scope.Resolve<PersonsMovieCreditsPageViewModel>(                    
                        new TypedParameter(typeof(PersonsDetailsModel), personDetails),
                        new TypedParameter(typeof(IPageService), new PageService(this))
                    );
            }           

            InitializeComponent();
        }

    }
}
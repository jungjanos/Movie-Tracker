using Ch9.Models;
using Ch9.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ch9
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PersonsMovieCreditsPage : ContentPage
    {
        public PersonsMovieCreditsPageViewModel ViewModel
        {
            get => BindingContext as PersonsMovieCreditsPageViewModel;
            set => BindingContext = value;
        }

        public PersonsMovieCreditsPage(GetPersonsMovieCreditsModel personsMovieCreditsModel)
        {
            ViewModel = new PersonsMovieCreditsPageViewModel(
                personsMovieCreditsModel,
                ((App)Application.Current).Settings,
                ((App)Application.Current).CachedSearchClient,                
                ((App)Application.Current).MovieDetailModelConfigurator,                
                new PageService(this)
                );

            InitializeComponent();
        }
    }

    public class ActorOrCrewBoolToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            (bool)value ? "As film crew" : "As actor";

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();        
    }
}
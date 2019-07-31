using Ch9.Models;
using Ch9.Utils;
using System;
using System.Globalization;
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

    public class ActorOrCrewBoolToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            (bool)value ? "As film crew" : "As actor";

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();        
    }

    public class StringToNotNullOremptyStringToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            !string.IsNullOrEmpty((value as string));

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) 
            => throw new NotImplementedException();
    }
}
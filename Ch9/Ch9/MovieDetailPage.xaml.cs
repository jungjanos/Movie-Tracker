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
                ((App)Application.Current).MovieDetailModelConfigurator,
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
    }

    // Converts the status of the movie in relation of the active MovieList to a Button color:
    // true => Movie is already on the list, hitting the btn will remove the Movie from the list => _negativeColor
    // false => Movie is not yet on the list, hitting the btn will add the Movie from the list => _positiveColor
    // null => no MovieList is selected as active => _invalidColor
    // Only one-way-binding is requred
    public class MovieOnListBoolToColorConverter : IValueConverter
    {
        private readonly Color _positiveColor = Color.Green;
        private readonly Color _negativeColor = Color.Red;
        private readonly Color _invalidColor = Color.DarkSlateGray;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((bool?)value)
            {
                case true: return _negativeColor;
                case false: return _positiveColor;
                case null: return _invalidColor;
            }
            throw new ArgumentException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class MovieHasReviewsBoolToColorConverter : IValueConverter
    {
        private readonly Color _positiveColor = Color.DarkGreen;
        private readonly Color _negativeColor = Color.LightSlateGray;
        private readonly Color _invalidColor = Color.LightGray;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((bool?)value)
            {
                case true: return _positiveColor;
                case false: return _negativeColor;
                case null: return _invalidColor;
            }
            throw new ArgumentException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


}
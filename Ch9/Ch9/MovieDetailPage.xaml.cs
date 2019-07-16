using Ch9.ApiClient;
using Ch9.Models;
using Ch9.Utils;
using System;
using System.Globalization;
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
                ((App)Application.Current).UsersMovieListsService2,
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
    // true => Movie is already on the list => _positiveColor
    // false => Movie is not yet on the list => _negativeColor
    // null => no MovieList is selected as active => _invalidColor
    // Only one-way-binding is requred
    public class MovieOnListBoolToColorConverter : IValueConverter
    {
        private readonly Color _positiveColor = Color.DarkGreen;
        private readonly Color _negativeColor = Color.DarkGray;
        private readonly Color _invalidColor = Color.Gray;

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

    // converts IsFavorite property from AccountMovieStates object to 
    // color value to be used on the favorite button UI control.
    public class MovieOnFavoriteListAccountMovieStatesToColorConverter : IValueConverter
    {
        private readonly Color _positiveColor = Color.DarkRed;
        private readonly Color _negativeColor = Color.DarkGray;
        private readonly Color _invalidColor = Color.Gray;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            AccountMovieStates states = value as AccountMovieStates;
            if (states == null)
                return _invalidColor;
            else if (states.IsFavorite)
                return _positiveColor;
            else return _negativeColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        { throw new NotImplementedException(); }
    }

    // converts IsFavorite property from AccountMovieStates object to 
    // Glyph icon code which is to be used on the favorite button UI control.
    public class MovieOnFavoriteListAccountMovieStatesToGlyphConverter : IValueConverter
    {
        private readonly string _positiveGlyph = "\uf308"; //ion-md-heart
        private readonly string _negativeGlyph = "\uf1a1"; //ion-md-heart-empty
        private readonly string _invalidGlyph = "\uf308"; //ion-md-heart

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            AccountMovieStates states = value as AccountMovieStates;
            if (states == null)
                return _invalidGlyph;
            else if (states.IsFavorite)
                return _positiveGlyph;
            else return _negativeGlyph;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        { throw new NotImplementedException(); }
    }

    // converts OnWatchlist property from AccountMovieStates object to 
    // color value to be used on the toggle watchlist button UI control.
    public class MovieOnWathlistAccountMovieStatesToColorConverter : IValueConverter
    {
        private readonly Color _positiveColor = Color.DarkGreen;
        private readonly Color _negativeColor = Color.DarkGray;
        private readonly Color _invalidColor = Color.Gray;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            AccountMovieStates states = value as AccountMovieStates;
            if (states == null)
                return _invalidColor;
            else if (states.OnWatchlist)
                return _positiveColor;
            else return _negativeColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        { throw new NotImplementedException(); }
    }
}
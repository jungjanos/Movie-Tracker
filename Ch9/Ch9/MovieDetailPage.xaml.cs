using Ch9.ApiClient;
using Ch9.Models;
using Ch9.Utils;
using FormsVideoLibrary;
using System;
using System.Globalization;
using System.Linq;
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
        { throw new NotImplementedException(); }
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

    public class UrlStringToVideoSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            VideoSource.FromUri(value as string);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        { throw new NotImplementedException(); }
    }

    /// <summary>
    /// Converts a zero based gallery array index to the first part of the text to display about the number of images,
    /// e.g. array position = 5 => "6/"
    /// </summary>
    public class IntPlusOneToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int index = (int)value + 1;
            return index > 0 ? index.ToString() + '/' : string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        { throw new NotImplementedException(); }
    }

    public class GalleryImageTypeToGlyphConverter : IValueConverter
    {
        private static readonly string _imagesGlyph = "\uf30f";
        private static readonly string _youtubeGlyph = "\uf34f";
        /// <summary>
        /// Converts a boolean GalleryImage type to a UI Glyph code representing either a video or a photo album icon
        /// </summary>
        /// <param name="value">false: image gallery, true: video thumbnail gallery</param>
        /// <returns>Glyph icon as string containing unicode glyph code </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (bool)value ? _youtubeGlyph : _imagesGlyph;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        { throw new NotImplementedException(); }
    }

    public class GalleryImageCountToColorConverter : IValueConverter
    {
        private readonly static Color _containsElements = Color.Red;
        private readonly static Color _empty = Color.DarkGray;

        /// <summary>
        /// Converts an integer GalleryImage Count property to a color value representing either a video or a photo album glyph icon's color
        /// </summary>
        /// <param name="value">Count property of the image collection</param>        
        /// <param name="parameter"> :</param>        
        /// <returns>Color value of the glyph icon</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return 0 < (int)value ? _containsElements : _empty;
            } catch { }
            return _empty;
        }            

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        { throw new NotImplementedException(); }
    }

    public class ShowCreditsToColorConverter : IValueConverter
    {
        public readonly Color _expanded = Color.Green;
        public readonly Color _closed = Color.Blue;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (bool)value ? _expanded : _closed;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        { throw new NotImplementedException(); }
    }
}
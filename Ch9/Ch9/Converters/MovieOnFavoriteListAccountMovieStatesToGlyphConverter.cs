using Ch9.Models;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace Ch9.Converters
{
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

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
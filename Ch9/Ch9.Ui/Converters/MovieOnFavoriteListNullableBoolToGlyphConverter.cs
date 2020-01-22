using System;
using System.Globalization;
using Xamarin.Forms;

namespace Ch9.Converters
{
    // converts IsFavorite property to Glyph icon code which is to be used on the favorite button UI control.
    public class MovieOnFavoriteListNullableBoolToGlyphConverter : IValueConverter
    {
        private readonly string _positiveGlyph = "\uf308"; //ion-md-heart
        private readonly string _negativeGlyph = "\uf1a1"; //ion-md-heart-empty
        private readonly string _invalidGlyph = "\uf308"; //ion-md-heart

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isFavorite= value as bool?;

            return isFavorite == null ? _invalidGlyph : isFavorite.Value ? _positiveGlyph : _negativeGlyph;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
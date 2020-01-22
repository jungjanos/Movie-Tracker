using System;
using System.Globalization;
using Xamarin.Forms;

namespace Ch9.Converters
{
    // converts IsFavorite property to 
    // color value to be used on the favorite button UI control.
    public class MovieOnFavoriteListNullableBoolToColorConverter : IValueConverter
    {
        private readonly Color _positiveColor = Color.DarkRed;
        private readonly Color _negativeColor = Color.DarkGray;
        private readonly Color _invalidColor = Color.Gray;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isFavorite = value as bool?;

            return isFavorite == null ? _invalidColor : isFavorite.Value ? _positiveColor : _negativeColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => 
            throw new NotImplementedException();
    }

}
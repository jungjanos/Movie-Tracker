using Ch9.Models;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace Ch9.Converters
{
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

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
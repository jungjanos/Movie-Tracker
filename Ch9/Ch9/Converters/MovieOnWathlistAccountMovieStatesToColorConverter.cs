using Ch9.Models;

using System;
using System.Globalization;
using Xamarin.Forms;

namespace Ch9.Converters
{
    // converts OnWatchlist property from AccountMovieStates object to 
    // color value to be used on the toggle watchlist button UI control.
    public class MovieOnWathlistAccountMovieStatesToColorConverter : IValueConverter
    {
        private readonly Color _positiveColor = Color.DarkGreen;
        private readonly Color _negativeColor = Color.DarkGray;
        private readonly Color _invalidColor = Color.DarkGray;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            AccountMovieStates states = value as AccountMovieStates;
            if (states == null)
                return _invalidColor;
            else if (states.OnWatchlist)
                return _positiveColor;
            else return _negativeColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
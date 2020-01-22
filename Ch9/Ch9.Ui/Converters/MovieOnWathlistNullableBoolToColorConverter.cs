using System;
using System.Globalization;
using Xamarin.Forms;

namespace Ch9.Converters
{
    // converts OnWatchlist property to color value to be used on the toggle watchlist button UI control.
    public class MovieOnWathlistNullableBoolToColorConverter : IValueConverter
    {
        private readonly Color _positiveColor = Color.DarkGreen;
        private readonly Color _negativeColor = Color.DarkGray;
        private readonly Color _invalidColor = Color.DarkGray;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var onWatchlist= value as bool?;

            return onWatchlist == null ? _invalidColor : onWatchlist.Value ? _positiveColor : _negativeColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
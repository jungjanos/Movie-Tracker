using System;
using System.Globalization;
using Xamarin.Forms;

namespace Ch9.Converters
{
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

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => 
            throw new NotImplementedException(); 
    }
}
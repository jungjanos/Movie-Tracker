using System;
using System.Globalization;
using Xamarin.Forms;

namespace Ch9.Converters
{
    public class IsLoggedinToLoginGlyphColorConverter : IValueConverter
    {
        private static Color _loginColor = Color.DarkGreen;
        private static Color _logoutColor = Color.DarkRed;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            (bool)value ? _logoutColor : _loginColor;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}

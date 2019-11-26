using System;
using System.Globalization;
using Xamarin.Forms;

namespace Ch9.Converters
{
    public class IsLoggedinToLoginGlyphBoolToGlyphConverter : IValueConverter
    {
        private static string _loginGlyph = "\uf1b1";
        private static string _logoutGlyph = "\uf1b2";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            (bool)value ? _logoutGlyph : _loginGlyph;        

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>        
            throw new NotImplementedException();        
    }
}

using System;
using System.Globalization;
using Xamarin.Forms;

namespace Ch9.Converters
{
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

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => 
            throw new NotImplementedException(); 
    }
}
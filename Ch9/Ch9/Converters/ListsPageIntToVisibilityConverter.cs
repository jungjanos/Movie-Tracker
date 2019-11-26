using System;
using System.Globalization;
using Xamarin.Forms;

namespace Ch9.Converters
{
    /// <summary>
    /// Converts the integer encoded type of the user list (1 = CUSTOM, 2 = FAVORITES 3 = WATCHLIST) to a bool visibility value used on the UI 
    /// the parameter is position (1 or 2 or 3) of the UI element
    /// </summary>    
    public class ListsPageIntToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            (int)value == (int)parameter;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
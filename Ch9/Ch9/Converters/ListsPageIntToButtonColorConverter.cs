using System;
using System.Globalization;
using Xamarin.Forms;

namespace Ch9.Converters
{
    /// <summary>
    /// Converts the integer encoded type of the user list (1 = CUSTOM, 2 = FAVORITES 3 = WATCHLIST) to a button color value used on the UI 
    /// the parameter is position (1 or 2 or 3) of the UI element
    /// </summary>
    public class ListsPageIntToButtonColorConverter : IValueConverter
    {
        private readonly Color _activeColor = Color.DarkGray;
        private readonly Color _nonActiveColor = Color.LightGray;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => 
            (int)value == (int)parameter ? _activeColor : _nonActiveColor;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>        
            throw new NotImplementedException();        
    }
}
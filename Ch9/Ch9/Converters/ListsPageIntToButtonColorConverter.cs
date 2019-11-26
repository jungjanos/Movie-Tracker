using System;
using System.Globalization;
using Xamarin.Forms;

namespace Ch9.Converters
{
    /// <summary>
    /// Converts the integer encoded type of the user list (1 = WATCHLIST, 2 = FAVORITES, 3 = CUSTOM) to a button color value used on the UI 
    /// the parameter is position (1 or 2 or 3) of the UI element
    /// </summary>
    public class ListsPageIntToButtonColorConverter : IValueConverter
    {
        private readonly Color _watchlistIsActive = Color.DarkGreen;
        private readonly Color _favoritelistIsActive = Color.DarkRed;
        private readonly Color _customListsAreActive = Color.DarkBlue;
        private readonly Color _nonActiveColor = Color.Gray;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((int)value != (int)parameter)
                return _nonActiveColor;

            if ((int)value == 1)
                return _watchlistIsActive;

            if ((int)value == 2)
                return _favoritelistIsActive;

            if ((int)value == 3)
                return _customListsAreActive;

            return _nonActiveColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
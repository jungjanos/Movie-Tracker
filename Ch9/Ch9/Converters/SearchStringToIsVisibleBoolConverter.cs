using System;
using System.Globalization;
using Xamarin.Forms;

namespace Ch9.Converters
{
    // returns whether or not the search text info (e.g. type at least # characters) should be displayed
    // search test info should be displayed if the user has already typed some characters, 
    // but not enought to initiate a search
    // the auxilary parameter is expected as integer and denotes the minimum allowed search length
    public class SearchStringToIsVisibleBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string txt = value as string;
            if (string.IsNullOrEmpty(txt))
                return false;

            return txt.Length < (int)parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => 
            throw new NotImplementedException(); 
    }
}
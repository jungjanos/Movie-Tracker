using FormsVideoLibrary;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace Ch9.Converters
{
    public class UrlStringToVideoSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            VideoSource.FromUri(value as string);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
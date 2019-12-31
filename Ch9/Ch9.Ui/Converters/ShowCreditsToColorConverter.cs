using System;
using System.Globalization;
using Xamarin.Forms;

namespace Ch9.Converters
{
    public class ShowCreditsToColorConverter : IValueConverter
    {
        public readonly Color _expanded = Color.Green;
        public readonly Color _closed = Color.Blue;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (bool)value ? _expanded : _closed;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
using System;
using System.Globalization;
using Xamarin.Forms;

namespace Ch9.Converters
{
    public class IntIntMultiplierConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (int)value * int.Parse(parameter as string ?? "0");

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>        
            throw new NotImplementedException();        
    }
}

using System;
using System.Globalization;
using Xamarin.Forms;

namespace Ch9.Converters
{
    public class RecommendationOrSimilarBoolToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (bool)value ? "Recommended" : "Similar";

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
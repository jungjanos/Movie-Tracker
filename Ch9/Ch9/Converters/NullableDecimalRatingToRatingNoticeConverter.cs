using System;
using System.Globalization;
using Xamarin.Forms;

namespace Ch9.Converters
{
    public class NullableDecimalRatingToRatingNoticeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            decimal? rating = value as decimal?;
            return rating.HasValue ? "Your rating: " : "Rate it!";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }

}
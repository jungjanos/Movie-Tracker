using System;
using System.Globalization;
using Xamarin.Forms;

namespace Ch9.Converters
{
    public class GalleryImageCountToColorConverter : IValueConverter
    {
        private readonly static Color _containsElements = Color.Red;
        private readonly static Color _empty = Color.DarkGray;

        /// <summary>
        /// Converts an integer GalleryImage Count property to a color value representing either a video or a photo album glyph icon's color
        /// </summary>
        /// <param name="value">Count property of the image collection</param>        
        /// <param name="parameter"> :</param>        
        /// <returns>Color value of the glyph icon</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return 0 < (int)value ? _containsElements : _empty;
            }
            catch { }
            return _empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
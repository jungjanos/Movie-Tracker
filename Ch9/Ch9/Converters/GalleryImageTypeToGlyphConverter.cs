using System;
using System.Globalization;
using Xamarin.Forms;

namespace Ch9.Converters
{
    public class GalleryImageTypeToGlyphConverter : IValueConverter
    {
        private static readonly string _imagesGlyph = "\uf30f";
        private static readonly string _youtubeGlyph = "\uf34f";
        /// <summary>
        /// Converts a boolean GalleryImage type to a UI Glyph code representing either a video or a photo album icon
        /// </summary>
        /// <param name="value">false: image gallery, true: video thumbnail gallery</param>
        /// <returns>Glyph icon as string containing unicode glyph code </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (bool)value ? _youtubeGlyph : _imagesGlyph;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => 
            throw new NotImplementedException(); 
    }
}
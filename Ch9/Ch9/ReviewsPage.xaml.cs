using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ch9
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReviewsPage : ContentPage
    {
        public ReviewsPageViewModel ViewModel
        {
            get => BindingContext as ReviewsPageViewModel;
            set => BindingContext = value;
        }

        public ReviewsPage(ReviewsPageViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
        }
    }

    public class RatingToGlyphConverter : IValueConverter
    {
        private readonly string _favoriteStar = "\ue734";
        private readonly string _favoriteStarFill = "\ue735";
        private readonly string _outlineQuarterStarLeft = "\uf0e5";
        private readonly string _outlineHalfStarLeft = "\uf0e7";
        private readonly string _outlineThreeQuarterStarLeft = "\uf0e9";

        public object Convert(object value, Type targetType, object glyphPosistion, CultureInfo culture)
        {
            int position = int.Parse((string)glyphPosistion);

            decimal? rating = value as decimal?;

            if (rating == null)
                return _favoriteStar;

            decimal starValue = rating.Value / 2;

            if (0 <= starValue - (position + 1))
                return _favoriteStarFill;

            if (0.75M <= starValue - (position))
                return _outlineThreeQuarterStarLeft;

            if (0.5M <= starValue - (position))
                return _outlineHalfStarLeft;

            if (0.25M <= starValue - (position))
                return _outlineQuarterStarLeft;

            return _favoriteStar;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        { throw new NotImplementedException(); }
    }

}
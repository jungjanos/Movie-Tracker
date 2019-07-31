using Ch9.Utils;
using System;
using System.Globalization;
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

        public ReviewsPage(MovieDetailPageViewModel parentViewModel)
        {
            ViewModel = new ReviewsPageViewModel(
                parentViewModel,
                ((App)Application.Current).Settings,
                ((App)Application.Current).CachedSearchClient,
                new PageService(this)
                );

            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await ViewModel.InitializeVM();
        }
    }

    // Converts a rating value represented as decimal? 
    // to a glyph icon which visualizes the state of a rating star 
    // as parameter object is expected as the 0-based position of the particular rating star
    // e.g. int 3 represents the 4-th star from left
    public class RatingToGlyphConverter : IValueConverter
    {
        private readonly string _favoriteStar = "\ue734";
        private readonly string _favoriteStarFill = "\ue735";
        private readonly string _outlineQuarterStarLeft = "\uf0e5";
        private readonly string _outlineHalfStarLeft = "\uf0e7";
        private readonly string _outlineThreeQuarterStarLeft = "\uf0e9";

        public object Convert(object value, Type targetType, object glyphPosition, CultureInfo culture)
        {
            int position = int.Parse((string)glyphPosition);

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
    
    public class NullableDecimalRatingToHasRatingConverter : IValueConverter 
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            decimal? rating = value as decimal?;
            return rating.HasValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {throw new NotImplementedException();}
    }

    public class NullableDecimalRatingToRatingNoticeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            decimal? rating = value as decimal?;
            return rating.HasValue ? "Your rating: " : "Rate it!";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        { throw new NotImplementedException(); }
    }

}
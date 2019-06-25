using Ch9.Utils;
using System;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ch9
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TrendingPage2 : ContentPage
    {
        public TrendingPage2ViewModel ViewModel
        {
            get => BindingContext as TrendingPage2ViewModel;
            set => BindingContext = value;
        }
        public TrendingPage2()
        {
            InitializeComponent();

            ViewModel = new TrendingPage2ViewModel(
                ((App)Application.Current).Settings,
                ((App)Application.Current).CachedSearchClient,
                ((App)Application.Current).ResultFilter,
                ((App)Application.Current).MovieDetailModelConfigurator,
                new PageService(this));
        }

        private void TrendingMovies_ItemTapped(object sender, ItemTappedEventArgs e) => ViewModel.ItemTappedCommand.Execute(e.Item);

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await ViewModel.Initialize();
        }
    }         

    public class WeekOrDayBoolToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (bool)value ? "Week" : "Day";

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
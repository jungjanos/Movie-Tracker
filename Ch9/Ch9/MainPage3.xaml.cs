using Ch9.Utils;
using System;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ch9
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage3 : ContentPage
    {
        public MainPage3ViewModel ViewModel
        {
            get => BindingContext as MainPage3ViewModel;
            set => BindingContext = value;
        }
        public MainPage3()
        {
            InitializeComponent();

            ViewModel = new MainPage3ViewModel(
                ((App)Application.Current).Settings,
                ((App)Application.Current).CachedSearchClient,
                ((App)Application.Current).ResultFilter,
                ((App)Application.Current).MovieDetailModelConfigurator,
                new PageService(this));
        }
    }

    // returns whether or not the search text info (e.g. type at least # characters) should be displayed
    // search test info should be displayed if the user has already typed some characters, 
    // but not enought to initiate a search
    // the auxilary parameter is expected as integer and denotes the minimum allowed search length
    public class SearchStringToIsVisibleBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string txt = value as string;
            if (string.IsNullOrEmpty(txt))
                return false;

            return txt.Length < (int)parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        { throw new NotImplementedException(); }
    }
}
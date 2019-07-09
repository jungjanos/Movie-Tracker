using Ch9.Utils;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ch9
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListsPage : ContentPage
    {
        Task vMInitializer;

        public ListsPageViewModel3 ViewModel
        {
            get => BindingContext as ListsPageViewModel3;
            set => BindingContext = value;
        }

        public ListsPage()
        {
            ViewModel = new ListsPageViewModel3(
                    ((App)Application.Current).UsersMovieListsService2,                                    
                    new PageService(this));

            vMInitializer = ViewModel.Initialize();
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            await vMInitializer;
            base.OnAppearing();
            
        }

        private void MovieListEntryTapped(object sender, ItemTappedEventArgs e) => ViewModel.MovieListEntryTappedCommand.Execute(e.Item);

    }

    /// <summary>
    /// Converts the integer encoded type of the user list (1 = CUSTOM, 2 = FAVORITES 3 = WATCHLIST) to a bool visibility value used on the UI 
    /// the parameter is position (1 or 2 or 3) of the UI element
    /// </summary>    
    public class ListsPageIntToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (int)value == (int)parameter;
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {throw new NotImplementedException();}
    }
}
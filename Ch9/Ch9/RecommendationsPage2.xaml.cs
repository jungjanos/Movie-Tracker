using Ch9.Models;
using Ch9.Utils;
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
    public partial class RecommendationsPage2 : ContentPage
    {
        public RecommendationsPage2ViewModel ViewModel
        {
            get => BindingContext as RecommendationsPage2ViewModel;
            set => BindingContext = value;
        }

        public RecommendationsPage2(MovieDetailModel movie)
        {
            InitializeComponent();

            ViewModel = new RecommendationsPage2ViewModel(
                movie,
                ((App)Application.Current).Settings,
                ((App)Application.Current).CachedSearchClient,
                ((App)Application.Current).ResultFilter,
                ((App)Application.Current).MovieDetailModelConfigurator,
                new PageService(this)
                );
        }

        private void OnRecommendationsListView_ItemTapped(object sender, ItemTappedEventArgs e) => ViewModel.ItemTappedCommand.Execute(e.Item);

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await ViewModel.Initialize();
        }
    }

    public class RecommendationOrSimilarBoolToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (bool)value ? "Recommended" : "Similar";

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();        
    }
}
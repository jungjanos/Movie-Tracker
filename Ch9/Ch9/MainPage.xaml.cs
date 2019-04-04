using Ch9.ApiClient;
using Ch9.Models;
using Ch9.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace Ch9
{
    /*
    Main page provides searching for movies with a live updated result list
    Results are filtered based on user preferences and only results which are 
    relevant are shown

    Search first starts after a pre-set number of characters are entered
    User can select any ListView item for details         
    */
    public partial class MainPage : ContentPage
    {
        private const int MINIMUM_Search_Str_Length = 5;
        private ObservableCollection<MovieDetailModel> _movies;        
        private Settings _settings;

        public string SearchString { get; set; }        

        public MainPage()
        {
            _settings = ((App)Application.Current).Settings;
            _movies = new ObservableCollection<MovieDetailModel>();

            InitializeComponent();
            listView.ItemsSource = _movies;
        }

        private async void OnSettingsButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MainSettingsPage());
        }

        private async void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = (e.NewTextValue as string).Trim();

            SetUiControlVisibility(searchText);

            if (searchText.Length >= MINIMUM_Search_Str_Length)
            {
                var searchResult = await ((App)Application.Current).CachedSearchClient.SearchByMovie(searchText, _settings.SearchLanguage, _settings.IncludeAdult);

                if (searchResult.HttpStatusCode.IsSuccessCode())
                {
                    var deserializedApiResponse = JsonConvert.DeserializeObject<SearchResult>(searchResult.Json);
                    var filteredResult = ((App)Application.Current).ResultFilter.FilterBySearchSettings(deserializedApiResponse.MovieDetailModels);

                    ((App)Application.Current).MovieDetailModelConfigurator.SetImageSrc(filteredResult);
                    ((App)Application.Current).MovieDetailModelConfigurator.SetGenreNamesFromGenreIds(filteredResult);
                    Utils.Utils.UpdateListviewCollection(_movies, filteredResult, new MovieModelComparer());
                }
            }
        }

        private void SetUiControlVisibility(string searchText)
        {
            listView.IsVisible = searchText.Length >= MINIMUM_Search_Str_Length;
            searchLabel.IsVisible = searchText.Length != 0 && searchText.Length < MINIMUM_Search_Str_Length;
        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            MovieDetailModel movie = e.Item as MovieDetailModel;

            Navigation.PushAsync(new MovieDetailPage(movie));
        }

        private async void Trending_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TrendingPage());
        }
    }
}

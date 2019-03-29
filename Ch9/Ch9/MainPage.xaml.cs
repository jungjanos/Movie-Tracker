using Ch9.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using static Ch9.ApiClient.TheMovieDatabaseClient;

namespace Ch9
{
    public partial class MainPage : ContentPage
    {
        private ObservableCollection<MovieDetailModel> movies;
        private Dictionary<string, Dictionary<int, MovieDetailModel>> queryCache;
        private Settings settings;

        public string SearchString { get; set; }        

        public MainPage()
        {
            settings = ((App)Application.Current).Settings;

            movies = new ObservableCollection<MovieDetailModel>();
            queryCache = new Dictionary<string, Dictionary<int, MovieDetailModel>>();

            InitializeComponent();
            listView.ItemsSource = movies;
        }

        private async void OnSettingsButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MainSettingsPage());
        }

        private async void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = (e.NewTextValue as string).Trim();

            if (searchText.Length < 5)
                listView.IsVisible = false;
            else
                listView.IsVisible = true;

            if (searchText.Length == 0 || searchText.Length >= 5)
                searchLabel.IsVisible = false;
            else
                searchLabel.IsVisible = true;

            if (searchText.Length >= 5)
            {               
                var searchResult = await ((App)Application.Current).movieGetter2(searchText, settings.SearchLanguage, settings.IncludeAdult);

                if ( 200 <= (int)searchResult.HttpStatusCode && (int)searchResult.HttpStatusCode < 300)
                {
                    var obj = JsonConvert.DeserializeObject<SearchResult>(searchResult.Json);
                    ((App)Application.Current).ImagePathConfiguratorUtil.SetImageSrc(obj.MovieDetailModels);
                    Utils.Utils.UpdateListviewCollection(movies, obj.MovieDetailModels, new MovieModelComparer());
                }
                    
            }
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

using Ch9.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;


namespace Ch9
{
    public partial class MainPage : ContentPage
    {
        private ObservableCollection<MovieDetailModel> movies;
        private Dictionary<string, Dictionary<int, MovieDetailModel>> queryCache;

        public string SearchString { get; set; }

        private string queryCacheString = null;

        public MainPage()
        {
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
                ApiClient.TheMovieDatabaseClient.SearchResult searchResult = null;

                try
                {                    

                    searchResult = await ((App)Application.Current).movieGetter(searchText);
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Exception", ex.Message, "OK");
                }

                if (searchResult.HttpStatusCode != System.Net.HttpStatusCode.OK)
                {
                    await DisplayAlert("Search error", $"Error code: {searchResult.HttpStatusCode.ToString()}", "Ok");
                }
                else
                {                   
                    Utils.Utils.UpdateListviewCollection(movies, searchResult.MovieDetailModels, new MovieModelComparer());
                }
            }
        }               

        private bool QueryNeeded(string searchString)
        {
            if (queryCacheString == null)
                return true;
            else
            {
                if (searchString.Contains(queryCacheString))
                    return false;
                else return true;
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

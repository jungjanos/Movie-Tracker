using Ch9.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Ch9.ApiClient.TheMovieDatabaseClient;

namespace Ch9
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TrendingPage : ContentPage
	{
        ObservableCollection<MovieModel> movies;

        public bool QueryTheWeek { get; set; } = true;

        Task<SearchResult> trendingThisWeekTask;
        Task<SearchResult> trendingThisDayTask;



        public TrendingPage ()
		{
            movies = new ObservableCollection<MovieModel>();
            trendingThisWeekTask = ((App)App.Current).trendingWeekGetter.Invoke();
            trendingThisDayTask = ((App)App.Current).trendingDayGetter.Invoke();

            InitializeComponent ();

            listView.ItemsSource = movies;


        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            MovieDetailModel movie = e.Item as MovieDetailModel;

            Navigation.PushAsync(new MovieDetailPage(movie));
        }

        private async void WeekOrDaySwitch_Toggled(object sender, ToggledEventArgs e)
        {
            weekOrDayLabel.Text = QueryTheWeek ? "Week" : "Day";
            QueryTheWeek = !QueryTheWeek;

            Task<SearchResult> search = QueryTheWeek ? trendingThisWeekTask : trendingThisDayTask;

            SearchResult result = null;

            try
            {
                result = await search;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Exception", ex.Message, "OK");
            }

            if (result.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                await DisplayAlert("Search error", $"Error code: {result.HttpStatusCode.ToString()}", "Ok");
            }
            else
            {
                movies.Clear();
                foreach (MovieModel movie in result.MovieDetailModels)
                    movies.Add(movie);
            }

        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            Task<SearchResult> search = QueryTheWeek ? trendingThisWeekTask : trendingThisDayTask;

            SearchResult result = null;

            try
            {
                result = await search;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Exception", ex.Message, "OK");
            }

            if (result.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                await DisplayAlert("Search error", $"Error code: {result.HttpStatusCode.ToString()}", "Ok");
            }
            else
            {
                movies.Clear();
                foreach (MovieModel movie in result.MovieDetailModels)
                    movies.Add(movie);

            }


        }
    }
}
using Ch9.Models;
using Newtonsoft.Json;
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
        private Settings settings;
        ObservableCollection<MovieModel> movies;

        public bool QueryTheWeek { get; set; } = true;

        Task<TrendingMoviesResult> trendingThisWeekTask;
        Task<TrendingMoviesResult> trendingThisDayTask;


        public TrendingPage ()
		{
            settings = ((App)Application.Current).Settings;
            movies = new ObservableCollection<MovieModel>();

            trendingThisWeekTask = ((App)Application.Current).TrendingMoviesGetter.Invoke(true, settings.SearchLanguage, settings.IncludeAdult, null);
            trendingThisDayTask = ((App)Application.Current).TrendingMoviesGetter.Invoke(false, settings.SearchLanguage, settings.IncludeAdult, null);                                 

            InitializeComponent();
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

            TrendingMoviesResult result = await (QueryTheWeek ? trendingThisWeekTask : trendingThisDayTask);

            if (200 <= (int)result.HttpStatusCode && (int)result.HttpStatusCode < 300)
            {
                string json = result.Json;
                SearchResult obj = JsonConvert.DeserializeObject<SearchResult>(json);
                ((App)Application.Current).MovieDetailModelConfigurator.SetImageSrc(obj.MovieDetailModels);
                ((App)Application.Current).MovieDetailModelConfigurator.SetGenreNamesFromGenreIds(obj.MovieDetailModels);
                movies.Clear();                
                foreach (MovieDetailModel movie in obj.MovieDetailModels)
                    movies.Add(movie);
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            TrendingMoviesResult result = await (QueryTheWeek ? trendingThisWeekTask : trendingThisDayTask);

            if (200 <= (int)result.HttpStatusCode && (int)result.HttpStatusCode < 300)
            {
                string json = result.Json;
                SearchResult obj = JsonConvert.DeserializeObject<SearchResult>(json);
                ((App)Application.Current).MovieDetailModelConfigurator.SetImageSrc(obj.MovieDetailModels);
                ((App)Application.Current).MovieDetailModelConfigurator.SetGenreNamesFromGenreIds(obj.MovieDetailModels);

                movies.Clear();
                foreach (MovieDetailModel movie in obj.MovieDetailModels)
                    movies.Add(movie);
            }
        }
    }
}
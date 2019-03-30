using Ch9.Models;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Ch9.ApiClient.TheMovieDatabaseClient;

namespace Ch9
{
    /*     
     This page displays trending movies for the week or day according to the bool QueryTheWeek switch
     To speed things up in the constructor parallel hot tasks are started to fetch week and day information.

     The results are filtered according to users preferences

     When switching between week and day query, the ListView is compleatly emptied and refilled from the 
     completed tasls to maintain the popularity sorted order in which the WebAPI sent it. At current 
     list sizes its not a UI bottleneck. 

        User can select any ListView item to view movie details and images         
     */

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
                var filteredResults = ((App)Application.Current).ResultFilter.FilterBySearchSettings(obj.MovieDetailModels);

                ((App)Application.Current).MovieDetailModelConfigurator.SetImageSrc(filteredResults);
                ((App)Application.Current).MovieDetailModelConfigurator.SetGenreNamesFromGenreIds(filteredResults);
                movies.Clear();                
                foreach (MovieDetailModel movie in filteredResults)
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
                var filteredResults = ((App)Application.Current).ResultFilter.FilterBySearchSettings(obj.MovieDetailModels);

                ((App)Application.Current).MovieDetailModelConfigurator.SetImageSrc(filteredResults);
                ((App)Application.Current).MovieDetailModelConfigurator.SetGenreNamesFromGenreIds(filteredResults);

                movies.Clear();
                foreach (MovieDetailModel movie in filteredResults)
                    movies.Add(movie);
            }
        }
    }
}
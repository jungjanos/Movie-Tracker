using Ch9.ApiClient;
using Ch9.Models;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ch9
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RecommendationsPage : ContentPage
	{
        
        ObservableCollection<MovieModel> _movies;
        Task<GetMovieRecommendationsResult> _getMovieRecommendations;
        Task<GetSimilarMoviesResult> _getSimilarMovies;

        public bool RecommendationsOrSimilars { get; set; } = true;
        public MovieDetailModel Movie { get; private set; }

        public RecommendationsPage (MovieDetailModel movie, Task<GetMovieRecommendationsResult> getMovieRecommendations, Task<GetSimilarMoviesResult> getSimilarMovies)
		{
            Movie = movie;
            _getMovieRecommendations = getMovieRecommendations;
            _getSimilarMovies = getSimilarMovies;
            _movies = new ObservableCollection<MovieModel>();

			InitializeComponent ();
            recommendationsListView.ItemsSource = _movies;
		}

        private void OnRecommendationsListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            MovieDetailModel movie = e.Item as MovieDetailModel;
            Navigation.PushAsync(new MovieDetailPage(movie));
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            GetMovieRecommendationsResult result = await _getMovieRecommendations;

            if (200 <= (int)result.HttpStatusCode && (int)result.HttpStatusCode < 300)
            {
                string json = result.Json;
                SearchResult deserializedApiResponse = JsonConvert.DeserializeObject<SearchResult>(json);
                var filteredResults = ((App)Application.Current).ResultFilter.FilterBySearchSettings(deserializedApiResponse.MovieDetailModels);

                ((App)Application.Current).MovieDetailModelConfigurator.SetImageSrc(filteredResults);
                ((App)Application.Current).MovieDetailModelConfigurator.SetGenreNamesFromGenreIds(filteredResults);

                _movies.Clear();
                foreach (MovieDetailModel movie in filteredResults)
                    _movies.Add(movie);
            }
        }

        private async void OnRecommendationsOrSimilarsSwitch_Toggled(object sender, ToggledEventArgs e)
        {           
            if (RecommendationsOrSimilars)
            {
                recommendationsOrSimilarsLabel.Text = "Recommended";

                var result = await _getMovieRecommendations;
                if(200 <= (int)result.HttpStatusCode && (int)result.HttpStatusCode < 300)
                {
                    string json = result.Json;
                    SearchResult deserializedApiResponse = JsonConvert.DeserializeObject<SearchResult>(json);
                    var filteredResults = ((App)Application.Current).ResultFilter.FilterBySearchSettings(deserializedApiResponse.MovieDetailModels);

                    ((App)Application.Current).MovieDetailModelConfigurator.SetImageSrc(filteredResults);
                    ((App)Application.Current).MovieDetailModelConfigurator.SetGenreNamesFromGenreIds(filteredResults);

                    _movies.Clear();
                    foreach (MovieDetailModel movie in filteredResults)
                        _movies.Add(movie);
                }
            }
            else
            {
                recommendationsOrSimilarsLabel.Text = "Similar";

                var result = await _getSimilarMovies;
                if (200 <= (int)result.HttpStatusCode && (int)result.HttpStatusCode < 300)
                {
                    string json = result.Json;
                    SearchResult deserializedApiResponse = JsonConvert.DeserializeObject<SearchResult>(json);
                    var filteredResults = ((App)Application.Current).ResultFilter.FilterBySearchSettings(deserializedApiResponse.MovieDetailModels);

                    ((App)Application.Current).MovieDetailModelConfigurator.SetImageSrc(filteredResults);
                    ((App)Application.Current).MovieDetailModelConfigurator.SetGenreNamesFromGenreIds(filteredResults);

                    _movies.Clear();
                    foreach (MovieDetailModel movie in filteredResults)
                        _movies.Add(movie);
                }
            }
        }
    }
}
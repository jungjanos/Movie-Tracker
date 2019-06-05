using Ch9.ApiClient;
using Ch9.Models;
using Ch9.Utils;
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

            GetMovieRecommendationsResult result = getMovieRecommendations.Result; // this Task has already finished (awaited in the parent page), its safe to call Task.Result
            AssembleListViewForUi(result);

            InitializeComponent();
            recommendationsListView.ItemsSource = _movies;
		}

        private void OnRecommendationsListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            MovieDetailModel movie = e.Item as MovieDetailModel;
            Navigation.PushAsync(new MovieDetailPage(movie));
        }

        private void AssembleListViewForUi(TmdbResponseBase result)
        {
            if (result.HttpStatusCode.IsSuccessCode())
            {                
                SearchResult deserializedApiResponse = JsonConvert.DeserializeObject<SearchResult>(result.Json);
                var filteredResults = ((App)Application.Current).ResultFilter.FilterBySearchSettings(deserializedApiResponse.MovieDetailModels);

                ((App)Application.Current).MovieDetailModelConfigurator.SetImageSrc(filteredResults);
                ((App)Application.Current).MovieDetailModelConfigurator.SetGenreNamesFromGenreIds(filteredResults);
                Utils.Utils.RefillList(_movies, filteredResults);
            }
        }

        private async void OnRecommendationsOrSimilarsSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            recommendationsOrSimilarsLabel.Text =  RecommendationsOrSimilars ?  "Recommended" : "Similar";

            if (RecommendationsOrSimilars)
            {               
                var result = await _getMovieRecommendations;
                AssembleListViewForUi(result);
            }
            else
            {              
                var result = await _getSimilarMovies;
                AssembleListViewForUi(result);
            }
        }
    }
}
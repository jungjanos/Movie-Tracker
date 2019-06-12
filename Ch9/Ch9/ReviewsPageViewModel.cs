using Ch9.ApiClient;
using Ch9.Models;
using Ch9.Utils;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Ch9
{
    public class ReviewsPageViewModel
    {
        private readonly MovieDetailPageViewModel _parent;
        private readonly ITmdbCachedSearchClient _cachedSearchClient;
        private readonly Task _initializer;

        public MovieDetailModel Movie { get; private set; }
        public ObservableCollection<Review> Reviews { get; private set; }
        public ReviewsPageViewModel(MovieDetailPageViewModel parent, ITmdbCachedSearchClient tmdbCachedSearchClient)
        {            
            _parent = parent;
            _cachedSearchClient = tmdbCachedSearchClient;

            Movie = _parent.Movie;            
            Reviews = new ObservableCollection<Review>();
            _initializer = Initialize();
        }

        private async Task Initialize()
        {
            var getReviewResult = await _cachedSearchClient.GetMovieReviews(Movie.Id, language: null, page: null, retryCount: 3, delayMilliseconds: 1000, fromCache: false);
            if (getReviewResult.HttpStatusCode.IsSuccessCode())
            {
                GetReviewsModel reviewsWrapper = JsonConvert.DeserializeObject<GetReviewsModel>(getReviewResult.Json);

                foreach (Review review in reviewsWrapper.Reviews)
                    Reviews.Add(review);
                _parent.MovieHasReviews = Reviews.Count > 0;
            }
        }
    }
}

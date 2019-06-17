using Ch9.ApiClient;
using Ch9.Models;
using Ch9.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Ch9
{
    public class ReviewsPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

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

        // This stupidity is required bc of the way the Server 
        // side wraps the rating into a variably type Json object.
        public decimal? UsersRating 
        {
            get => _parent.MovieStates.Rating?.Value;
            set
            {
                if (value == UsersRating)
                    return;

                if (value == null)
                    _parent.MovieStates.Rating = null;
                else
                    _parent.MovieStates.Rating.Value = value.Value;

                OnPropertyChanged(nameof(UsersRating));
            }
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
        private void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}

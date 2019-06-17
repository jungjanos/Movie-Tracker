using Ch9.ApiClient;
using Ch9.Models;
using Ch9.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Ch9
{
    public class ReviewsPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly MovieDetailPageViewModel _parent;
        private readonly ITmdbCachedSearchClient _cachedSearchClient;
        private readonly Task _initializer;

        public ICommand DecreaseRatingCommand { get; private set; }
        public ICommand IncreaseRatingCommand { get; private set; }

        public MovieDetailModel Movie { get; private set; }
        public ObservableCollection<Review> Reviews { get; private set; }
        public ReviewsPageViewModel(MovieDetailPageViewModel parent, ITmdbCachedSearchClient tmdbCachedSearchClient)
        {
            _parent = parent;
            _cachedSearchClient = tmdbCachedSearchClient;

            Movie = _parent.Movie;            

            DecreaseRatingCommand = new Command(async () => await OnDecreaseRatingCommand());
            IncreaseRatingCommand = new Command(async () => await OnIncreaseRatingCommand());

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
                {
                    if (_parent.MovieStates.Rating == null)
                        _parent.MovieStates.Rating = new RatingWrapper();

                    _parent.MovieStates.Rating.Value = value.Value;
                }   

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

        private async Task OnDecreaseRatingCommand()
        {
            decimal newRating = UsersRating == null ? 4.5M : Math.Max(UsersRating.Value - 0.5M, 0.5M);

            if (UsersRating == newRating)
                return;

            bool success = await UpdateRating(newRating);

            if (success)
                UsersRating = newRating;
        }

        private async Task OnIncreaseRatingCommand()
        {
            decimal newRating = UsersRating == null ? 5.5M : Math.Min(UsersRating.Value + 0.5M, 10M);

            if (UsersRating == newRating)
                return;

            bool success = await UpdateRating(newRating);

            if (success)
                UsersRating = newRating;
        }

        private async Task<bool> UpdateRating(decimal targetRating)
        {
            int enumValue = (int)(targetRating * 2);

            Rating rating = (Rating)enumValue;

            var response = await _cachedSearchClient.RateMovie(rating, Movie.Id, null, 3, 1000);

            return response.HttpStatusCode.IsSuccessCode();
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

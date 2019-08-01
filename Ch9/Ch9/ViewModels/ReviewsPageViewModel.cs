using Ch9.ApiClient;
using Ch9.Models;
using Ch9.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Ch9.ViewModels
{
    public class ReviewsPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly ISettings _settings;
        private readonly ITmdbCachedSearchClient _cachedSearchClient;
        private readonly IPageService _pageService;

        public ICommand DeleteRatingCommand { get; private set; }
        public ICommand SetRatingCommand { get; private set; }
        public ICommand DecreaseRatingCommand { get; private set; }
        public ICommand IncreaseRatingCommand { get; private set; }
        public MovieDetailPageViewModel ParentPageViewModel { get; set; }

        public ReviewsPageViewModel(MovieDetailPageViewModel parent, ISettings settings, ITmdbCachedSearchClient tmdbCachedSearchClient, IPageService pageService)
        {            
            _cachedSearchClient = tmdbCachedSearchClient;
            _pageService = pageService;

            ParentPageViewModel = parent;
            _settings = settings;
            DeleteRatingCommand = new Command(async () => await OnDeleteRatingCommand());
            SetRatingCommand = new Command<string>(async str => {
                decimal targetRating = decimal.Parse(str);
                await OnSetRatingCommand(targetRating);
            });
            DecreaseRatingCommand = new Command(async () => await OnDecreaseRatingCommand());
            IncreaseRatingCommand = new Command(async () => await OnIncreaseRatingCommand());
        }

        // This stupidity is required bc the way the Server 
        // side wraps the rating into a variably typed Json object.
        public decimal? UsersRating
        {
            get => ParentPageViewModel.MovieStates?.Rating?.Value;
            set
            {
                if (value == UsersRating)
                    return;

                // parents 'MovieStates' object CAN BE NULL
                if (ParentPageViewModel.MovieStates == null)
                    return;

                if (value == null)
                    ParentPageViewModel.MovieStates.Rating = null;
                else
                {
                    if (ParentPageViewModel.MovieStates.Rating == null)
                        ParentPageViewModel.MovieStates.Rating = new RatingWrapper();

                    ParentPageViewModel.MovieStates.Rating.Value = value.Value;
                }
                OnPropertyChanged(nameof(UsersRating));
            }
        }

        /// <summary>
        /// Must be called when the View is appearing. Initializes the Review collection if not already done or if it is empty.
        /// </summary>        
        public async Task InitializeVM()
        {
            if (! (ParentPageViewModel.Movie.Reviews?.Count > 0))
                await RefreshReviews(retryCount: 1, delayMilliseconds: 1000, fromCache: false);
        }

        private async Task RefreshReviews(int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = false )
        {
            var getReviewResult = await _cachedSearchClient.GetMovieReviews(ParentPageViewModel.Movie.Id, language: null, page: null, retryCount: retryCount, delayMilliseconds: delayMilliseconds, fromCache: fromCache);
            if (getReviewResult.HttpStatusCode.IsSuccessCode())
            {
                GetReviewsModel reviewsWrapper = JsonConvert.DeserializeObject<GetReviewsModel>(getReviewResult.Json);

                ParentPageViewModel.Movie.Reviews = new List<Review>(reviewsWrapper.Reviews);
            }
        }

        private async Task OnSetRatingCommand(decimal targetRating)
        {
            bool success = await UpdateRating(targetRating);
            if (success)
                UsersRating = targetRating;
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

        public async Task OnDeleteRatingCommand()
        {
            if (UsersRating == null)
                return;
            var response = await _cachedSearchClient.DeleteMovieRating(ParentPageViewModel.Movie.Id, null, retryCount: 3, delayMilliseconds: 1000);

            if (response.HttpStatusCode.IsSuccessCode())
                UsersRating = null;
            else
                await _pageService.DisplayAlert("Error", $"Could not delete your rating, server reponse: {response.HttpStatusCode}", "Ok");
        }

        private async Task<bool> UpdateRating(decimal targetRating)
        {
            if(!_settings.HasTmdbAccount)
            {
                await _pageService.DisplayAlert("Info", $"To vote, You need to log in with a user account", "Ok");
                return false;
            }                

            try
            {
                int enumValue = (int)(targetRating * 2);
                Rating rating = (Rating)enumValue;

                var response = await _cachedSearchClient.RateMovie(rating, ParentPageViewModel.Movie.Id, null, 1, 1000);

                if (!response.HttpStatusCode.IsSuccessCode())
                    await _pageService.DisplayAlert("Error", $"Could not update your vote, server response: {response.HttpStatusCode}", "Ok");

                return response.HttpStatusCode.IsSuccessCode();
            }
            catch (Exception ex)
            { await _pageService.DisplayAlert("Error", $"Could not update your vote, exception happened: {ex.Message}", "Ok"); }

            return false;
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

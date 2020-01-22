using Ch9.Services;
using Ch9.Models;
using Ch9.Services.Contracts;
using Ch9.Infrastructure.Extensions;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Ch9.ViewModels
{
    public class ReviewsPageViewModel : ViewModelBase
    {
        private readonly ISettings _settings;        
        private readonly ITmdbApiService _tmdbApiService;
        public ICommand DeleteRatingCommand { get; private set; }
        public ICommand SetRatingCommand { get; private set; }
        public ICommand DecreaseRatingCommand { get; private set; }
        public ICommand IncreaseRatingCommand { get; private set; }
        public MovieDetailPageViewModel ParentPageViewModel { get; set; }

        public ReviewsPageViewModel(MovieDetailPageViewModel parent, ISettings settings, ITmdbApiService tmdbApiService, IPageService pageService) : base(pageService)
        {            
            _tmdbApiService = tmdbApiService;
            ParentPageViewModel = parent;
            _settings = settings;
            DeleteRatingCommand = new Command(async () => await OnDeleteRatingCommand());
            SetRatingCommand = new Command<string>(async str =>
            {
                decimal targetRating = decimal.Parse(str);
                await OnSetRatingCommand(targetRating);
            });
            DecreaseRatingCommand = new Command(async () => await OnDecreaseRatingCommand());
            IncreaseRatingCommand = new Command(async () => await OnIncreaseRatingCommand());


            // Ensures that the Review collection is populated if not already done or if it is empty.
            Func<Task> initializationAction = async () =>
            {
                if (!(ParentPageViewModel.Movie.Reviews?.Count > 0))
                    await RefreshReviews(retryCount: 1, delayMilliseconds: 1000, fromCache: false);
            };

            ConfigureInitialization(initializationAction, false);
        }

        private async Task RefreshReviews(int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = false)
        {
            var response = await _tmdbApiService.TryGetMovieReviews(ParentPageViewModel.Movie.Id, language: null, page: null, retryCount: retryCount, delayMilliseconds: delayMilliseconds, fromCache: fromCache);
            if (response.HttpStatusCode.IsSuccessCode())
            {
                ReviewsModel reviewsModel = response.ReviewsModel;
                ParentPageViewModel.Movie.Reviews = new List<Review>(reviewsModel.Reviews);
            }
        }

        private async Task OnSetRatingCommand(decimal targetRating)
        {
            bool success = await UpdateRating(targetRating);
            if (success)
                ParentPageViewModel.UsersRating = targetRating;
        }

        private async Task OnDecreaseRatingCommand()
        {
            decimal newRating = ParentPageViewModel.UsersRating == null ? 4.5M : Math.Max(ParentPageViewModel.UsersRating.Value - 0.5M, 0.5M);

            if (ParentPageViewModel.UsersRating == newRating)
                return;

            bool success = await UpdateRating(newRating);

            if (success)
                ParentPageViewModel.UsersRating = newRating;
        }

        private async Task OnIncreaseRatingCommand()
        {
            decimal newRating = ParentPageViewModel.UsersRating == null ? 5.5M : Math.Min(ParentPageViewModel.UsersRating.Value + 0.5M, 10M);

            if (ParentPageViewModel.UsersRating == newRating)
                return;

            bool success = await UpdateRating(newRating);

            if (success)
                ParentPageViewModel.UsersRating = newRating;
        }

        public async Task OnDeleteRatingCommand()
        {
            if (ParentPageViewModel.UsersRating == null)
                return;

            var response = await _tmdbApiService.TryDeleteMovieRating(ParentPageViewModel.Movie.Id, null, retryCount: 3, delayMilliseconds: 1000);
            
            if (response.HttpStatusCode.IsSuccessCode())
                ParentPageViewModel.UsersRating = null;
            else
                await _pageService.DisplayAlert("Error", $"Could not delete your rating, server reponse: {response.HttpStatusCode}", "Ok");
        }

        private async Task<bool> UpdateRating(decimal targetRating)
        {
            if (!_settings.IsLoggedin)
            {
                await _pageService.DisplayAlert("Info", $"To vote, You need to log in with a user account", "Ok");
                return false;
            }

            try
            {
                var response = await _tmdbApiService.TryRateMovie(targetRating, ParentPageViewModel.Movie.Id, null, 1, 1000);
                if (!response.HttpStatusCode.IsSuccessCode())
                    await _pageService.DisplayAlert("Error", $"Could not update your vote, server response: {response.HttpStatusCode}", "Ok");

                return response.HttpStatusCode.IsSuccessCode();
            }
            catch (Exception ex)
            { await _pageService.DisplayAlert("Error", $"Could not update your vote, exception happened: {ex.Message}", "Ok"); }

            return false;
        }
    }
}

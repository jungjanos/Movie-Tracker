using Ch9.Services;
using Ch9.Services.Contracts;

using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Ch9.ViewModels
{
    public class ReviewsPageViewModel : ViewModelBase
    {              
        private readonly IRatingService _ratingService;

        public ICommand DeleteRatingCommand { get; private set; }
        public ICommand SetRatingCommand { get; private set; }
        public ICommand DecreaseRatingCommand { get; private set; }
        public ICommand IncreaseRatingCommand { get; private set; }
        public MovieDetailPageViewModel ParentPageViewModel { get; set; }

        public ReviewsPageViewModel(
            MovieDetailPageViewModel parent,            
            IRatingService ratingService,
            IPageService pageService) : base(pageService)
        {
            _ratingService = ratingService;
            ParentPageViewModel = parent;     
            DeleteRatingCommand = new Command(async () => await OnDeleteRatingCommand());
            SetRatingCommand = new Command<string>(async str =>
            {
                decimal targetRating = decimal.Parse(str);
                await OnSetRatingCommand(targetRating);
            });
            DecreaseRatingCommand = new Command(async () => await OnDecreaseRatingCommand());
            IncreaseRatingCommand = new Command(async () => await OnIncreaseRatingCommand());

            // Ensures that the Review collection is populated
            ConfigureInitialization(async () =>{ParentPageViewModel.Movie.Reviews = await _ratingService.TryFetchMovieReviewsFirstPage(ParentPageViewModel.Movie.Id, retryCount: 1, delayMilliseconds: 1000);}, true);
        }

        private async Task OnSetRatingCommand(decimal targetRating) => await UpdateRating(targetRating);

        private async Task OnDecreaseRatingCommand()
        {
            decimal newRating = ParentPageViewModel.UsersRating == null ? 4.5M : Math.Max(ParentPageViewModel.UsersRating.Value - 0.5M, 0.5M);

            await UpdateRating(newRating);
        }

        private async Task OnIncreaseRatingCommand()
        {
            decimal newRating = ParentPageViewModel.UsersRating == null ? 5.5M : Math.Min(ParentPageViewModel.UsersRating.Value + 0.5M, 10M);

            await UpdateRating(newRating);
        }

        public async Task OnDeleteRatingCommand()
        {
            if (ParentPageViewModel.UsersRating == null)
                return;

            try
            {
                await _ratingService.DeleteRating(ParentPageViewModel.Movie.Id, retryCount: 2, delayMilliseconds: 1000);
                ParentPageViewModel.UsersRating = null;
            }
            catch (Exception ex)
            { await _pageService.DisplayAlert("Error", ex.Message, "Ok"); }
        }

        private async Task UpdateRating(decimal targetRating)
        {
            if (targetRating == ParentPageViewModel.UsersRating)
                return;

            try
            {
                await _ratingService.SetRating(targetRating, movieId: ParentPageViewModel.Movie.Id, retryCount: 1, delayMilliseconds: 1000);
                ParentPageViewModel.UsersRating = targetRating;
            }
            catch (Exception ex)
            { await _pageService.DisplayAlert("Error", ex.Message, "Ok"); }            
        }
    }
}

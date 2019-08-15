using Ch9.Models;
using Ch9.ViewModels;
using Ch9.Views;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Ch9.Services
{
    // IPageService is injected into the ViewModel objects in order to access 
    // UI-page functions (navigation, page instantiation)
    public interface IPageService
    {
        Task<string> DisplayActionSheet(string title, string cancel, string destruction, params string[] buttons);
        Task DisplayAlert(string title, string message, string cancel);
        Task<bool> DisplayAlert(string title, string message, string accept, string cancel);
        Task OpenWeblink(string url);
        Task<object> PopCurrent();
        Task PopToRootAsync();
        Task PushAsync(MovieDetailModel movie);
        Task PushAsync(AddListPageViewModel viewModel);
        Task PushLargeImagePageAsync(MovieDetailPageViewModel viewModel);
        Task PushPersonsMovieCreditsPageAsync(GetPersonsDetailsModel personDetails);
        Task PushRecommendationsPageAsync(MovieDetailModel movie);
        Task PushReviewsPage(MovieDetailPageViewModel model);        
        Task PushVideoPageAsync(ImageModel videoThumbnailWithVideo);
    }

    public class PageService : IPageService
    {
        private readonly Page _currentPage;

        public PageService(Page current) => _currentPage = current;

        public async Task PushAsync(MovieDetailModel movie) => await _currentPage.Navigation.PushAsync(new MovieDetailPage(movie));

        public async Task PushReviewsPage(MovieDetailPageViewModel model) =>
            await _currentPage.Navigation.PushAsync(new ReviewsPage(model));

        public async Task PushAsync(AddListPageViewModel viewModel) =>
            await _currentPage.Navigation.PushAsync(new AddListPage(viewModel));

        public async Task PushRecommendationsPageAsync(MovieDetailModel movie) =>
            await _currentPage.Navigation.PushAsync(new RecommendationsPage3(movie));

        public async Task PushLargeImagePageAsync(MovieDetailPageViewModel viewModel) =>
            await _currentPage.Navigation.PushAsync(new LargeImagePage(viewModel));

        public async Task PushVideoPageAsync(ImageModel videoThumbnailWithVideo) =>        
            await _currentPage.Navigation.PushAsync(new VideoPage(videoThumbnailWithVideo));
        

        public async Task PushPersonsMovieCreditsPageAsync(GetPersonsDetailsModel personDetails) =>
            await _currentPage.Navigation.PushAsync(new PersonsMovieCreditsPage(personDetails));

        public async Task<string> DisplayActionSheet(string title, string cancel, string destruction, params string[] buttons) =>
             await _currentPage.DisplayActionSheet(title, cancel, destruction, buttons);

        public async Task DisplayAlert(string title, string message, string cancel) =>
            await _currentPage.DisplayAlert(title, message, cancel);

        public async Task<bool> DisplayAlert(string title, string message, string accept, string cancel) =>
            await _currentPage.DisplayAlert(title, message, accept, cancel);

        public async Task<object> PopCurrent()
        {
            await _currentPage.Navigation.PopAsync();
            return _currentPage.BindingContext;
        }

        public async Task PopToRootAsync() => await _currentPage.Navigation.PopToRootAsync();

        public async Task OpenWeblink(string url)
        {
            try
            {
                Device.OpenUri(new Uri(url));
            }
            catch (Exception ex)
            {
                await _currentPage.DisplayAlert("Error", $"Could not open weblink: {ex.Message}", "Ok");
            }

        }
    }
}

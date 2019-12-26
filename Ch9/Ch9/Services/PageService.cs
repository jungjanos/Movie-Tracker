using Ch9.Ui.Contracts.Models;
using Ch9.ViewModels;
using Ch9.Views;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
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
        Task OpenMovieGenreSelection();
        Task OpenWeblink(string url);
        Task<object> PopCurrent();
        Task PopToRootAsync();
        Task PushAddListPageAsync(ListsPageViewModel3 listsPageViewModel);
        Task PushAsync(MovieDetailModel movie);
        Task PushLargeImagePageAsync(MovieDetailPageViewModel viewModel);
        Task PushLoginPageAsync(string accountName = null, string password = null);
        Task PushPersonsMovieCreditsPageAsync(PersonsDetailsModel personDetails);
        Task PushRecommendationsPageAsync(MovieDetailModel movie);
        Task PushReviewsPage(MovieDetailPageViewModel model);
        Task PushVideoPageAsync(string streamUrl);
    }

    public class PageService : IPageService
    {
        private readonly Page _currentPage;

        public PageService(Page current) => _currentPage = current;

        public async Task PushAsync(MovieDetailModel movie) => await _currentPage.Navigation.PushAsync(new MovieDetailPage(movie));

        public async Task PushReviewsPage(MovieDetailPageViewModel model) =>
            await _currentPage.Navigation.PushAsync(new ReviewsPage(model));

        public async Task PushAddListPageAsync(ListsPageViewModel3 listsPageViewModel) =>
            await _currentPage.Navigation.PushAsync(new AddListPage(listsPageViewModel));

        public async Task PushRecommendationsPageAsync(MovieDetailModel movie) =>
            await _currentPage.Navigation.PushAsync(new RecommendationsPage3(movie));

        public async Task PushLargeImagePageAsync(MovieDetailPageViewModel viewModel) =>
            await _currentPage.Navigation.PushAsync(new LargeImagePage(viewModel));

        public async Task PushVideoPageAsync(string streamUrl) =>
            await _currentPage.Navigation.PushAsync(new VideoPage(streamUrl));

        public async Task PushPersonsMovieCreditsPageAsync(PersonsDetailsModel personDetails) =>
            await _currentPage.Navigation.PushAsync(new PersonsMovieCreditsPage(personDetails));

        public async Task PushLoginPageAsync(string accountName = null, string password = null) =>
            await _currentPage.Navigation.PushAsync(new LoginPage(accountName, password));

        public async Task OpenMovieGenreSelection() =>
            await _currentPage.Navigation.PushAsync(new GenreSettingsPage2());

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
                await Browser.OpenAsync(url, BrowserLaunchMode.SystemPreferred);
            }
            catch (Exception ex)
            {
                await _currentPage.DisplayAlert("Error", $"Could not open weblink: {ex.Message}", "Ok");
            }
        }
    }
}

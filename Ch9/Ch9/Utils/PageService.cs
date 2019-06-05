using Ch9.ApiClient;
using Ch9.Models;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Ch9.Utils
{
    public interface IPageService
    {
        Task<string> DisplayActionSheet(string title, string cancel, string destruction, params string[] buttons);
        Task DisplayAlert(string title, string message, string cancel);
        Task<bool> DisplayAlert(string title, string message, string accept, string cancel);
        Task<object> PopCurrent();
        Task PopToRootAsync();
        Task PushAsync(MovieDetailModel movie);
        Task PushAsync(AddListPageViewModel viewModel);
        Task PushRecommendationsPageAsync(MovieDetailModel movie, Task<GetMovieRecommendationsResult> getMovieRecommendations, Task<GetSimilarMoviesResult> getSimilarMovies);
    }

    public class PageService : IPageService
    {
        private readonly Page _currentPage;

        public PageService(Page current)
        {
            _currentPage = current;
        }

        public async Task PushAsync(MovieDetailModel movie)
        {
            await _currentPage.Navigation.PushAsync(new MovieDetailPage2(movie));
        }

        public async Task PushAsync(AddListPageViewModel viewModel)
        {
            await _currentPage.Navigation.PushAsync(new AddListPage(viewModel));
        }

        public async Task PushRecommendationsPageAsync(MovieDetailModel movie, Task<GetMovieRecommendationsResult> getMovieRecommendations, Task<GetSimilarMoviesResult> getSimilarMovies)
        {
            await _currentPage.Navigation.PushAsync(new RecommendationsPage(movie, getMovieRecommendations, getSimilarMovies));
        }


        public async Task<string> DisplayActionSheet(string title, string cancel, string destruction, params string[] buttons)
        {
            return await _currentPage.DisplayActionSheet(title, cancel, destruction, buttons);
        }

        public async Task DisplayAlert(string title, string message, string cancel)
        {
            await _currentPage.DisplayAlert(title, message, cancel);
        }

        public async Task<bool> DisplayAlert(string title, string message, string accept, string cancel)
        {
            return await _currentPage.DisplayAlert(title, message, accept, cancel);
        }

        public async Task<object> PopCurrent()
        {
            await _currentPage.Navigation.PopAsync();
            return _currentPage.BindingContext;
        }

        public async Task PopToRootAsync()
        {
            await _currentPage.Navigation.PopToRootAsync();
        }
    }
}

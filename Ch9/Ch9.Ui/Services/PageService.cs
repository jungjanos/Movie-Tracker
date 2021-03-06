﻿using Ch9.Models;
using Ch9.ViewModels;
using Ch9.Views;

using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Ch9.Services
{
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

        /// <summary>
        /// Tries to open a non-null or empty Url
        /// </summary>        
        public async Task OpenWeblink(string url)
        {
            if (string.IsNullOrEmpty(url))
                return;

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

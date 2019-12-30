using Ch9.Models;
using Ch9.ViewModels;

using System.Threading.Tasks;

namespace Ch9.Services
{
    // IPageService is injected into the ViewModel objects in order to access 
    // functions to control the UI (navigation, error messages) without a direct dependency on the UI (Xamarin page)
    public interface IPageService
    {
        Task<string> DisplayActionSheet(string title, string cancel, string destruction, params string[] buttons);
        Task DisplayAlert(string title, string message, string cancel);
        Task<bool> DisplayAlert(string title, string message, string accept, string cancel);
        Task OpenMovieGenreSelection();
        Task OpenWeblink(string url);
        Task<object> PopCurrent();
        Task PopToRootAsync();
        Task PushAsync(MovieDetailModel movie);
        Task PushLoginPageAsync(string accountName = null, string password = null);
        Task PushPersonsMovieCreditsPageAsync(PersonsDetailsModel personDetails);
        Task PushRecommendationsPageAsync(MovieDetailModel movie);        
        Task PushLargeImagePageAsync(MovieDetailPageViewModel viewModel);
        Task PushAddListPageAsync(ListsPageViewModel3 listsPageViewModel);
        Task PushReviewsPage(MovieDetailPageViewModel model);
    }
}

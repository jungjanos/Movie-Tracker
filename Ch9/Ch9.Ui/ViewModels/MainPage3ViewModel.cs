using Ch9.Services;
using Ch9.Models;
using Ch9.Services.Contracts;
using Ch9.Services.MovieListServices;

using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Ch9.ViewModels
{
    public class MainPage3ViewModel : ViewModelBase
    {
        private const int MINIMUM_Search_Str_Length = 3;

        private readonly IMovieSearchService _movieSearchService;
        private string _searchString;
        public string SearchString
        {
            get => _searchString;
            set
            {
                if (SetProperty(ref _searchString, value))
                {
                    if (string.IsNullOrEmpty(SearchString))
                        SearchResults.InitializeOrClearMovieCollection();
                }
            }
        }

        private SearchResult _searchResults;
        public SearchResult SearchResults
        {
            get => _searchResults;
            set => SetProperty(ref _searchResults, value);
        }

        public ICommand SearchCommand { get; private set; }
        public ICommand LoadNextResultPageCommand { get; private set; }
        public ICommand OnItemTappedCommand { get; private set; }

        public MainPage3ViewModel(IMovieSearchService movieSearchService, IPageService pageService) : base(pageService)
        {            
            _movieSearchService = movieSearchService;
            _searchResults = new SearchResult();
            _searchResults.InitializeOrClearMovieCollection();

            SearchCommand = new Command(async () =>
            {
                if (!(SearchString?.Length >= MINIMUM_Search_Str_Length))
                    return;

                SearchResults.InitializeOrClearMovieCollection();
                await TryLoadingNextResultPage(1, 1000);
            });

            LoadNextResultPageCommand = new Command(async () =>
            {
                if (!(SearchString?.Length >= MINIMUM_Search_Str_Length))
                    return;

                await TryLoadingNextResultPage();
            });

            OnItemTappedCommand = new Command<MovieDetailModel>(async movie => await _pageService.PushAsync(movie));
        }


        // Tries to load the next page of the result set
        private async Task TryLoadingNextResultPage(int retryCount = 0, int delayMilliseconds = 1000)
        {
            if (SearchResults.Page == 0 || SearchResults.Page < SearchResults.TotalPages)
            {
                IsBusy = true;
                try
                {
                    var page = await _movieSearchService.LoadResultPage(SearchString, SearchResults.Page + 1, retryCount: retryCount, delayMilliseconds: delayMilliseconds, fromCache: true);
                    SearchResults.AppendResult(page);
                }
                catch (Exception ex) { await _pageService.DisplayAlert("Error", ex.Message, "Ok"); }
                finally { IsBusy = false; }
            }
        }
    }
}

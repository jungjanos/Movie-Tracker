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
    public class MainPage3ViewModel : INotifyPropertyChanged
    {
        private const int MINIMUM_Search_Str_Length = 3;

        private readonly ISettings _settings;
        private readonly ITmdbCachedSearchClient _cachedSearchClient;
        private readonly ISearchResultFilter _resultFilter;
        private readonly IMovieDetailModelConfigurator _movieDetailModelConfigurator;
        private readonly IPageService _pageService;

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

        private bool _isRefreshing = false;      
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public ICommand SearchCommand { get; private set; }
        public ICommand LoadNextResultPageCommand { get; private set; }
        public ICommand OnItemTappedCommand { get; private set; }

        public MainPage3ViewModel(ISettings settings,
            ITmdbCachedSearchClient cachedSearchClient,
            ISearchResultFilter resultFilter,
            IMovieDetailModelConfigurator movieDetailModelConfigurator,
            IPageService pageService)
        {
            _settings = settings;
            _cachedSearchClient = cachedSearchClient;
            _resultFilter = resultFilter;
            _movieDetailModelConfigurator = movieDetailModelConfigurator;
            _pageService = pageService;

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

        public async Task TryLoadingNextResultPage(int retryCount = 0, int delayMilliseconds = 1000)
        {
            if (SearchResults.Page == 0 || SearchResults.Page < SearchResults.TotalPages)
            {
                IsRefreshing = true;
                try
                {
                    var getNextPageResponse = await _cachedSearchClient.SearchByMovie(searchString: SearchString, _settings.SearchLanguage, _settings.IncludeAdult, SearchResults.Page + 1, year: null, retryCount, delayMilliseconds, fromCache: true);
                    if (!getNextPageResponse.HttpStatusCode.IsSuccessCode())
                    {
                        await _pageService.DisplayAlert("Error", $"Could not load search results, service responded with: {getNextPageResponse.HttpStatusCode}", "Ok");
                        return;
                    }
                    SearchResult moviesOnNextPage = JsonConvert.DeserializeObject<SearchResult>(getNextPageResponse.Json);

                    var filteredResults =  _settings.IncludeAdult ?  _resultFilter.FilterBySearchSettingsIncludeAdult(moviesOnNextPage.MovieDetailModels) : _resultFilter.FilterBySearchSettings(moviesOnNextPage.MovieDetailModels);

                    moviesOnNextPage.MovieDetailModels = new ObservableCollection<MovieDetailModel>(filteredResults);

                    Utils.Utils.AppendResult(SearchResults, moviesOnNextPage, _movieDetailModelConfigurator);
                }
                catch (Exception ex) { await _pageService.DisplayAlert("Error", $"Could not load search results, service responded with: {ex.Message}", "Ok"); }
                finally { IsRefreshing = false; }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

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

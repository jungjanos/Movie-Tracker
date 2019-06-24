using Ch9.ApiClient;
using Ch9.Models;
using Ch9.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Ch9
{
    public class MainPage2ViewModel : INotifyPropertyChanged
    {   
        private const int MINIMUM_Search_Str_Length = 4;

        private readonly ISettings _settings;
        private readonly ITmdbCachedSearchClient _cachedSearchClient;
        private readonly IMovieDetailModelConfigurator _movieDetailModelConfigurator;
        private readonly IPageService _pageService;
        private ObservableCollection<MovieDetailModel> _searchResults;
        public ObservableCollection<MovieDetailModel> SearchResults
        {
            get => _searchResults;
            set => SetProperty(ref _searchResults, value);
        }

        private string _searchString;
        public string SearchString
        {
            get => _searchString;
            set
            {
                if (SetProperty(ref _searchString, value))
                {
                    if (string.IsNullOrEmpty(SearchString))
                        SearchResults.Clear();
                }
            } 
        }

        public ICommand SearchCommand { get; private set; }
        public ICommand ItemTappedCommand { get; private set; }

        public MainPage2ViewModel(
            ISettings settings, 
            ITmdbCachedSearchClient cachedSearchClient,
            IMovieDetailModelConfigurator movieDetailModelConfigurator,
            IPageService pageService)
        {
            _settings = settings;
            _cachedSearchClient = cachedSearchClient;
            _movieDetailModelConfigurator = movieDetailModelConfigurator;
            _pageService = pageService;

            SearchCommand = new Command(async () => await OnSearchCommand());
            ItemTappedCommand = new Command<int>(async id => await OnItemTappedCommand(id));
        }

        private async Task OnItemTappedCommand(int movieId)
        {
            await _pageService.PushAsync(SearchResults.First(movie => movie.Id == movieId));
        }

        private async Task OnSearchCommand()
        {
            var searchResult = await _cachedSearchClient.SearchByMovie(SearchString, _settings.SearchLanguage, _settings.IncludeAdult);

            if (searchResult.HttpStatusCode.IsSuccessCode())
            {
                try
                {
                    var deserializedApiResponse = JsonConvert.DeserializeObject<SearchResult>(searchResult.Json);
                    var filteredResult = ((App)Application.Current).ResultFilter.FilterBySearchSettings(deserializedApiResponse.MovieDetailModels).ToList();

                    _movieDetailModelConfigurator.SetImageSrc(filteredResult);
                    _movieDetailModelConfigurator.SetGenreNamesFromGenreIds(filteredResult);

                    SearchResults = new ObservableCollection<MovieDetailModel>(filteredResult);
                }
                catch (Exception ex)
                {
                    await _pageService.DisplayAlert("Error", $"The following exception was thrown: {ex.Message}", "Ok");
                }
            }
            else
                await _pageService.DisplayAlert("Network error", $"Server responded with the following error code: {searchResult.HttpStatusCode}", "Ok");
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

using Ch9.Models;
using Ch9.Services.Contracts;
using Ch9.Infrastructure.Extensions;

using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Ch9.Services
{
    public class MovieSearchService : IMovieSearchService
    {
        private readonly ISettings _settings;
        private readonly ITmdbApiService _tmdbApiService;
        private readonly ISearchResultFilter _resultFilter;
        private readonly IMovieDetailModelConfigurator _movieDetailModelConfigurator;

        public MovieSearchService(
            ISettings settings,
            ITmdbApiService tmdbApiService,
            ISearchResultFilter resultFilter,
            IMovieDetailModelConfigurator movieDetailModelConfigurator)
        {
            _settings = settings;
            _tmdbApiService = tmdbApiService;
            _resultFilter = resultFilter;
            _movieDetailModelConfigurator = movieDetailModelConfigurator;
        }

        /// <summary>
        /// Tries to fetch the requested page of the search results from the server. 
        /// </summary>
        public async Task<SearchResult> LoadResultPage(string searchString, int pageToLoad, int retryCount, int delayMilliseconds, bool fromCache)
        {
            var pageResponse = await _tmdbApiService.TrySearchByMovie(searchString: searchString, _settings.SearchLanguage, !_settings.SafeSearch, pageToLoad, year: null, retryCount, delayMilliseconds, fromCache: fromCache);
            if (pageResponse.HttpStatusCode.IsSuccessCode())
            {
                var searchResult = pageResponse.SearchResult;

                var filteredResults = _settings.SafeSearch ? _resultFilter.FilterBySearchSettings(searchResult.MovieDetailModels) : _resultFilter.FilterBySearchSettingsIncludeAdult(searchResult.MovieDetailModels);
                searchResult.MovieDetailModels = new ObservableCollection<MovieDetailModel>(filteredResults);
                _movieDetailModelConfigurator.SetImageSrc(searchResult.MovieDetailModels);
                _movieDetailModelConfigurator.SetGenreNamesFromGenreIds(searchResult.MovieDetailModels);

                return searchResult;
            }
            else
                throw new Exception($"Could not load search results, service responded with: {pageResponse.HttpStatusCode}");
        }
    }
}

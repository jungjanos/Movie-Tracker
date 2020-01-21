using Ch9.Models;
using Ch9.Services.Contracts;
using Ch9.Infrastructure.Extensions;

using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Ch9.Services
{
    public class TrendingMoviesService : ITrendingMoviesService
    {
        private readonly ISettings _settings;
        private readonly ITmdbApiService _tmdbApiService;
        private readonly ISearchResultFilter _resultFilter;
        private readonly IMovieDetailModelConfigurator _movieDetailConfigurator;

        public TrendingMoviesService(
            ISettings settings,
            ITmdbApiService tmdbApiService,
            ISearchResultFilter resultFilter,
            IMovieDetailModelConfigurator movieDetailConfigurator)
        {
            _settings = settings;
            _tmdbApiService = tmdbApiService;
            _resultFilter = resultFilter;
            _movieDetailConfigurator = movieDetailConfigurator;
        }

        /// <summary>
        /// Tries to fetch the requested page of the trending movie list from the server. 
        /// </summary>
        public async Task<SearchResult> LoadTrendingPage(bool week, int page, int retryCount, int delayMilliseconds)
        {
            var pageResponse = await _tmdbApiService.TryGetTrendingMovies(week: week, _settings.SearchLanguage, !_settings.SafeSearch, page, retryCount, delayMilliseconds);

            if (pageResponse.HttpStatusCode.IsSuccessCode())
            {
                var result = pageResponse.TrendingMovies;

                result.MovieDetailModels = new ObservableCollection<MovieDetailModel>(_resultFilter.FilterBySearchSettings(result.MovieDetailModels));

                _movieDetailConfigurator.SetImageSrc(result.MovieDetailModels);
                _movieDetailConfigurator.SetGenreNamesFromGenreIds(result.MovieDetailModels);

                return result;
            }
            else
            {
                var timeSpan = week ? "weeks" : "days";
                throw new Exception($"Could not load the {timeSpan} trending list, service responded with: {pageResponse.HttpStatusCode}");
            }
        }
    }
}

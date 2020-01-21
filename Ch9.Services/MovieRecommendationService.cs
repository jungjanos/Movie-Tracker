using Ch9.Models;
using Ch9.Services.Contracts;
using Ch9.Infrastructure.Extensions;

using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Ch9.Services
{
    public class MovieRecommendationService : IMovieRecommendationService
    {
        private readonly ISettings _settings;
        private readonly ITmdbApiService _tmdbApiService;
        private readonly ISearchResultFilter _searchResultFilter;
        private readonly IMovieDetailModelConfigurator _movieDetailModelConfigurator;

        public MovieRecommendationService(
            ISettings settings,
            ITmdbApiService tmdbApiService,
            ISearchResultFilter searchResultFilter,
            IMovieDetailModelConfigurator movieDetailModelConfigurator)
        {
            _settings = settings;
            _tmdbApiService = tmdbApiService;
            _searchResultFilter = searchResultFilter;
            _movieDetailModelConfigurator = movieDetailModelConfigurator;
        }

        public async Task<SearchResult> LoadSimilarMoviesPage(int movieId, int page, int retryCount, int delayMilliseconds)
        {
            var response = await _tmdbApiService.TryGetSimilarMovies(movieId, _settings.SearchLanguage, page, retryCount, delayMilliseconds);

            if (response.HttpStatusCode.IsSuccessCode())
            {
                var result = response.SimilarMovies;
                result.MovieDetailModels = new ObservableCollection<MovieDetailModel>(_searchResultFilter.FilterBySearchSettings(result.MovieDetailModels));

                _movieDetailModelConfigurator.SetImageSrc(result.MovieDetailModels);
                _movieDetailModelConfigurator.SetGenreNamesFromGenreIds(result.MovieDetailModels);

                return result;
            }
            else
                throw new Exception($"Could not update the similar movies list, service responded with: {response.HttpStatusCode}");
        }

        public async Task<SearchResult> LoadMovieRecommendationsPage(int movieId, int page, int retryCount, int delayMilliseconds)
        {
            var response = await _tmdbApiService.TryGetMovieRecommendations(movieId, _settings.SearchLanguage, page, retryCount, delayMilliseconds);

            if (response.HttpStatusCode.IsSuccessCode())
            {
                var result = response.MovieRecommendations;
                result.MovieDetailModels = new ObservableCollection<MovieDetailModel>(_searchResultFilter.FilterBySearchSettings(result.MovieDetailModels));

                _movieDetailModelConfigurator.SetImageSrc(result.MovieDetailModels);
                _movieDetailModelConfigurator.SetGenreNamesFromGenreIds(result.MovieDetailModels);

                return result;
            }
            else
                throw new Exception($"Could not update the similar movies list, service responded with: {response.HttpStatusCode}");
        }
    }
}

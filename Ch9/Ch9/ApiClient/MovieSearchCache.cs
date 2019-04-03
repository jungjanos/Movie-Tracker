using Ch9.ApiClient;
using LazyCache;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static Ch9.ApiClient.TheMovieDatabaseClient;

namespace Ch9.ApiClient
{
    public class MovieSearchCache
    {
        private IAppCache _cache;
        private TheMovieDatabaseClient _apiClient;

        public MovieSearchCache(TheMovieDatabaseClient theMovieDatabaseClient)
        {
            _cache = new CachingService();
            _apiClient = theMovieDatabaseClient;
        }

        public async Task<SearchByMovieResult> SearchByMovie(string searchString, string language = null, bool? includeAdult = null, int? page = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            return await _cache.GetOrAddAsync("$" + nameof(SearchByMovie) + searchString + (language ?? "") + 
                (includeAdult?.ToString() ?? ""), () => _apiClient.SearchByMovie(searchString, language, includeAdult));            
        }

        //ToDo: extract FetchMovieDetailsResult class declaration
        public async Task<FetchMovieDetailsResult> FetchMovieDetails(int id, string language = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            return await _cache.GetOrAddAsync("$" + nameof(FetchMovieDetails)  + id.ToString() + (language ?? ""), () => _apiClient.FetchMovieDetails(id, language));            
        }

        //ToDo: extract TrendingMoviesResult class declaration
        public async Task<TrendingMoviesResult> GetTrendingMovies(bool week = true, string language = null, bool? includeAdult = null, int? page = null, int retryCount = 0, int delayMilliseconds = 1000)
        {
            return await _cache.GetOrAddAsync("$" + nameof(GetTrendingMovies) + week + (language ?? "") + (includeAdult?.ToString() ?? "") + (page?.ToString() ?? ""), () =>
               _apiClient.GetTrendingMovies(week, language, includeAdult));
        }

        //ToDo: extract GetMovieImagesResult class declaration
        public async Task<GetMovieImagesResult> UpdateMovieImages(int id, string language = null, string otherLanguage = null, bool? includeLanguageless = true, int retryCount = 0, int delayMilliseconds = 1000)
        {
            return await _cache.GetOrAddAsync("$" + nameof(UpdateMovieImages) + id.ToString() + (language ?? "") + (otherLanguage ?? "") + (includeLanguageless == null ? "" : includeLanguageless.Value.ToString()),

                    () => _apiClient.UpdateMovieImages(id, language, otherLanguage, includeLanguageless));
        }
    }
}

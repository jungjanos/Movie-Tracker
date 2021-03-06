﻿using Ch9.Models;

using System.Net;
using System.Threading.Tasks;

namespace Ch9.Services.Contracts
{
    public interface ITmdbApiService
    {
        Task<TryAddMovieResponse> TryAddMovie(int listId, int mediaId, int retryCount = 0, int delayMilliseconds = 1000);
        Task<TryCreateListResponse> TryCreateList(string name, string description, string language = "en", int retryCount = 0, int delayMilliseconds = 1000);
        Task<TryCreateRequestTokenResponse> TryCreateRequestToken(int retryCount = 0, int delayMilliseconds = 1000);
        Task<TryCreateSessionIdResponse> TryCreateSessionId(string requestToken, int retryCount = 0, int delayMilliseconds = 1000);
        Task<TryDeleteListResponse> TryDeleteList(int listId, int retryCount = 0, int delayMilliseconds = 1000);
        Task<TryDeleteMovieRatingResponse> TryDeleteMovieRating(int mediaId, string guestSessionId = null, int retryCount = 0, int delayMilliseconds = 1000);
        Task<TryDeleteSessionResponse> TryDeleteSession(string sessionId, int retryCount = 0, int delayMilliseconds = 1000);
        Task<TryGetFavoriteMoviesResponse> TryGetFavoriteMovies(int? accountId = null, string language = null, string sortBy = null, int? page = null, int retryCount = 0, int delayMilliseconds = 1000);
        Task<TryGetGenreIdsWithNamesResponse> TryGetGenreIdsWithNames(string language = null, int retryCount = 0, int delayMilliseconds = 1000);
        Task<TryGetListDetailsResponse> TryGetListDetails(int listId, string language = null, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true);
        Task<TryGetListsResponse> TryGetLists(int? accountId = null, string language = null, int? page = null, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true);
        Task<TryGetMovieCreditsResponse> TryGetMovieCredits(int id, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true);
        Task<HttpStatusCode> TryGetMovieDetails(int id, string language = null, int retryCount = 0, int delayMilliseconds = 1000);
        Task<TryGetMovieDetailsWithAccountStatesResponse> TryGetMovieDetailsWithAccountStates(MovieDetailModel movieToPopulate, int id, string language = null, int retryCount = 0, int delayMilliseconds = 1000);
        Task<TryGetMovieImagesResponse> TryGetMovieImages(int id, string language = null, string otherLanguage = null, bool? includeLanguageless = true, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true);
        Task<TryGetMovieRecommendationsResponse> TryGetMovieRecommendations(int id, string language = null, int? page = null, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true);
        Task<TryGetMovieReviewsResponse> TryGetMovieReviews(int id, string language = null, int? page = null, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true);
        Task<TryGetMovieVideosResponse> TryGetMovieVideos(int id, string language = null, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true);
        Task<TryGetMovieWatchlistResponse> TryGetMovieWatchlist(int? accountId = null, string language = null, string sortBy = null, int? page = null, int retryCount = 0, int delayMilliseconds = 1000);
        Task<TryGetPersonImagesResponse> TryGetPersonImages(int id, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true);
        Task<TryGetPersonsDetailsResponse> TryGetPersonsDetails(int personId, string language = null, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true);
        Task<TryGetPersonsMovieCreditsResponse> TryGetPersonsMovieCredits(int personId, string language = null, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true);
        Task<TryGetSimilarMoviesResponse> TryGetSimilarMovies(int id, string language = null, int? page = null, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true);
        Task<TryGetTmdbConfigurationResponse> TryGetTmdbConfiguration(int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true);
        Task<TryGetTrendingMoviesResponse> TryGetTrendingMovies(bool week = true, string language = null, bool? includeAdult = null, int? page = null, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true);
        Task<TryRateMovieResponse> TryRateMovie(decimal rating, int mediaId, string guestSessionId = null, int retryCount = 0, int delayMilliseconds = 1000);
        Task<TryRemoveMovieResponse> TryRemoveMovie(int listId, int mediaId, int retryCount = 0, int delayMilliseconds = 1000);
        Task<TrySearchByMovieResponse> TrySearchByMovie(string searchString, string language = null, bool? includeAdult = null, int? page = null, int? year = null, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true);
        Task<TryUpdateFavoriteListResponse> TryUpdateFavoriteList(string mediaType, bool add, int mediaId, int? accountId = null, int retryCount = 0, int delayMilliseconds = 1000);
        Task<TryUpdateWatchlistResponse> TryUpdateWatchlist(string mediaType, bool add, int mediaId, int? accountId = null, int retryCount = 0, int delayMilliseconds = 1000);
        Task<TryValidateRequestTokenWithLoginResponse> TryValidateRequestTokenWithLogin(string username, string password, string requestToken, int retryCount = 0, int delayMilliseconds = 1000);
    }
}

﻿using System.Threading.Tasks;

namespace Ch9.Data.Contracts
{
    public interface ITmdbCachedSearchClient
    {
        Task<AddMovieResult> AddMovie(string sessionId, int listId, int mediaId, int retryCount = 0, int delayMilliseconds = 1000);
        Task<CreateListResult> CreateList(string sessionId, string name, string description, string language = "en", int retryCount = 0, int delayMilliseconds = 1000);
        Task<DeleteListResult> DeleteList(string sessionId, int listId, int retryCount = 0, int delayMilliseconds = 1000);
        Task<DeleteMovieRatingResult> DeleteMovieRating(string sessionId, int mediaId, string guestSessionId = null, int retryCount = 0, int delayMilliseconds = 1000);
        Task<GenreNameFetchResult> GetGenreIdsWithNames(string language = null, int retryCount = 0, int delayMilliseconds = 1000);
        Task<GetMovieDetailsResult> GetMovieDetails(int id, string language = null, int retryCount = 0, int delayMilliseconds = 1000);
        Task<GetMovieDetailsWithAccountStatesResult> GetMovieDetailsWithAccountStates(string sessionId, int id, string language = null, int retryCount = 0, int delayMilliseconds = 1000);
        Task<GetFavoriteMoviesResult> GetFavoriteMovies(string sessionId, int? accountId = null, string language = null, string sortBy = null, int? page = null, int retryCount = 0, int delayMilliseconds = 1000);
        Task<GetListDetailsResult> GetListDetails(string sessionId, int listId, string language = null, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true);
        Task<GetListsResult> GetLists(string sessionId, int? accountId = null, string language = null, int? page = null, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true);
        Task<GetMovieRecommendationsResult> GetMovieRecommendations(int id, string language = null, int? page = null, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true);
        Task<GetMovieReviewsResult> GetMovieReviews(int id, string language = null, int? page = null, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true);
        Task<GetMovieWatchlistResult> GetMovieWatchlist(string sessionId, int? accountId = null, string language = null, string sortBy = null, int? page = null, int retryCount = 0, int delayMilliseconds = 1000);
        Task<GetSimilarMoviesResult> GetSimilarMovies(int id, string language = null, int? page = null, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true);
        Task<TmdbConfigurationModelResult> GetTmdbConfiguration(int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true);
        Task<TrendingMoviesResult> GetTrendingMovies(bool week = true, string language = null, bool? includeAdult = null, int? page = null, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true);
        Task<RateMovieResult> RateMovie(string sessionId, decimal rating, int mediaId, string guestSessionId = null, int retryCount = 0, int delayMilliseconds = 1000);
        Task<RemoveMovieResult> RemoveMovie(string sessionId, int listId, int mediaId, int retryCount = 0, int delayMilliseconds = 1000);
        Task<SearchByMovieResult> SearchByMovie(string searchString, string language = null, bool? includeAdult = null, int? page = null, int? year = null, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true);
        Task<UpdateFavoriteListResult> UpdateFavoriteList(string sessionId, string mediaType, bool add, int mediaId, int? accountId = null, int retryCount = 0, int delayMilliseconds = 1000);
        Task<GetMovieImagesResult> GetMovieImages(int id, string language = null, string otherLanguage = null, bool? includeLanguageless = true, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true);
        Task<UpdateWatchlistResult> UpdateWatchlist(string sessionId, string mediaType, bool add, int mediaId, int? accountId = null, int retryCount = 0, int delayMilliseconds = 1000);
        Task<GetMovieVideosResult> GetMovieVideos(int id, string language = null, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true);
        Task<GetMovieCreditsResult> GetMovieCredits(int id, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true);
        Task<GetPersonsMovieCreditsResult> GetPersonsMovieCredits(int personId, string language = null, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true);
        Task<GetPersonsDetailsResult> GetPersonsDetails(int personId, string language = null, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true);
        Task<GetAccountMovieStatesResult> GetAccountMovieStates(string sessionId, int mediaId, string guestSessionId = null, int retryCount = 0, int delayMilliseconds = 1000);
        Task<GetItemStatusOnTargetListResult> GetItemStatusOnTargetList(int listId, int movieId, int retryCount = 0, int delayMilliseconds = 1000);
        Task<GetPersonImagesResult> GetPersonImages(int id, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true);
        Task<DeleteSessionResult> DeleteSession(string sessionId, int retryCount = 0, int delayMilliseconds = 1000);
        Task<CreateRequestTokenResult> CreateRequestToken(int retryCount = 0, int delayMilliseconds = 1000);
        Task<CreateRequestTokenResult> ValidateRequestTokenWithLogin(string username, string password, string requestToken, int retryCount = 0, int delayMilliseconds = 1000);
        Task<CreateSessionIdResult> CreateSessionId(string requestToken, int retryCount = 0, int delayMilliseconds = 1000);
    }
}
using System;
using System.Net;

namespace Ch9.Data.Contracts
{
    public class TmdbResponseBase
    {
        public HttpStatusCode HttpStatusCode { get; set; }
        public string Json { get; set; }
    }

    // TmdbResponseBase's derived classes use the same result format,
    // inheritacne is used only to give a more descriptive class names
    // and provide information about the API call used to get the results

    public class TmdbConfigurationModelResult : TmdbResponseBase
    { }

    public class GenreNameFetchResult : TmdbResponseBase
    { }

    public class SearchByMovieResult : TmdbResponseBase
    { }

    public class TrendingMoviesResult : TmdbResponseBase
    { }

    public class GetMovieDetailsResult : TmdbResponseBase
    { }

    public class GetMovieDetailsWithAccountStatesResult : TmdbResponseBase
    { }

    public class GetMovieImagesResult : TmdbResponseBase
    { }

    public class GetPersonImagesResult : TmdbResponseBase
    { }

    public class GetMovieVideosResult : TmdbResponseBase
    { }

    public class GetMovieRecommendationsResult : TmdbResponseBase
    { }

    public class GetSimilarMoviesResult : TmdbResponseBase
    { }

    public class CreateRequestTokenResult : TmdbResponseBase
    { }

    public class CreateSessionIdResult : TmdbResponseBase
    { }

    public class DeleteSessionResult : TmdbResponseBase
    { }

    public class GetAccountDetailsResult : TmdbResponseBase
    { }

    public class GetListsResult : TmdbResponseBase
    { }

    public class CreateListResult : TmdbResponseBase
    { }

    public class DeleteListResult : TmdbResponseBase
    { }

    public class GetListDetailsResult : TmdbResponseBase
    { }

    public class AddMovieResult : TmdbResponseBase
    { }

    public class RemoveMovieResult : TmdbResponseBase
    { }

    public class GetMovieReviewsResult : TmdbResponseBase
    { }

    public class GetMovieWatchlistResult : TmdbResponseBase
    { }

    public class GetFavoriteMoviesResult : TmdbResponseBase
    { }

    public class UpdateWatchlistResult : TmdbResponseBase
    { }

    public class UpdateFavoriteListResult : TmdbResponseBase
    { }

    public class RateMovieResult : TmdbResponseBase
    { }

    public class DeleteMovieRatingResult : TmdbResponseBase
    { }

    public class GetMovieCreditsResult : TmdbResponseBase
    { }

    public class GetPersonsMovieCreditsResult : TmdbResponseBase
    { }

    public class GetPersonsDetailsResult : TmdbResponseBase
    { }

    public class GetItemStatusOnTargetListResult : TmdbResponseBase
    { }

    /// <summary>
    /// Because of the custom deserialization requirement (the WebAPI sends back flexible object type as response)
    /// The deserialization is built into the response object itself. This behavior diverges from normal case
    /// </summary>
    public class GetAccountMovieStatesResult : TmdbResponseBase
    {
        /// <summary>
        /// Can trow. 
        /// If the HttpStatusCode property is 200(OK) then it tries to deserialize the Json response into 
        /// the model.
        /// </summary>
        /// <returns>object containing the account's states for a particular movie (rating, on watchlit, on favorite list) </returns>

        public GetAccountMovieStatesResult()
        {
            throw new NotImplementedException();
        }
    }
}

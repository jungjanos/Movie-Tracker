using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net;

namespace Ch9.ApiClient
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

    public class FetchMovieDetailsResult : TmdbResponseBase
    { }

    public class GetMovieImagesResult : TmdbResponseBase
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

    public class GetAccountMovieStatesResult2 : TmdbResponseBase
    {
        /// <summary>
        /// Can trow. 
        /// If the HttpStatusCode property is 200(OK) then it tries to deserialize the Json response into 
        /// the model.
        /// </summary>
        /// <returns>object containing the account's states for a particular movie (rating, on watchlit, on favorite list) </returns>
        public AccountMovieStates DeserializeJsonIntoModel()
        {
            if (HttpStatusCode != HttpStatusCode.OK)
                return null;

            var jsonSettings = new JsonSerializerSettings()
            {
                Error = delegate (object sender, ErrorEventArgs args) { args.ErrorContext.Handled = true; }
            };

            return JsonConvert.DeserializeObject<AccountMovieStates>(Json, jsonSettings);
        }
    }

    // Because of the flexible object type of the Json object the WebAPI sends back as response,
    // The Api client itself handles the deserialization of the server's response.
    // this behavior diverges from normal case
    public class GetAccountMovieStatesResult : TmdbResponseBase
    {
        public AccountMovieStates States { get; set; }
    }

    public enum Rating
    {
        Half = 1,
        One = 2,
        OneAndHalf = 3,
        Two = 4,
        TwoAndHalf = 5,
        Three = 6,
        ThreeAndHalf = 7,
        Four = 8,
        FourAndHalf = 9,
        Five = 10,
        FiveAndHalf = 11,
        Six = 12,
        SixAndHalf = 13,
        Seven = 14,
        SevenAndHalf = 15,
        Eight = 16,
        EightAndHalf = 17,
        Nine = 18,
        NineAndHalf = 19,
        Ten = 20
    }

    public class AccountMovieStates
    {
        [JsonProperty("id")]
        public int MovieId { get; set; }

        [JsonProperty("favorite")]
        public bool IsFavorite { get; set; }

        [JsonProperty("rated")]
        public RatingWrapper Rating { get; set; }
        [JsonProperty("watchlist")]
        public bool OnWatchlist { get; set; }
    }

    public class RatingWrapper
    {
        [JsonProperty("value")]
        public decimal Value { get; set; }
    }
}

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Ch9.Models
{
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

    // the dynamic JSON sent by the server is so stupid, that 
    // special care needs to be taken when handled, thus this DTO class 
    public class AccountMovieStatesDto
    {
        public bool? IsFavorite { get; private set; }
        public bool? OnWatchlist { get; private set; }
        public decimal? Rating { get; private set; } 

        public AccountMovieStatesDto(AccountMovieStates accountMovieStates)
        {
            IsFavorite = accountMovieStates?.IsFavorite;
            OnWatchlist = accountMovieStates?.OnWatchlist;
            Rating = accountMovieStates?.Rating?.Value;
        }
    }

}

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Ch9.Ui.Contracts.Models
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
}

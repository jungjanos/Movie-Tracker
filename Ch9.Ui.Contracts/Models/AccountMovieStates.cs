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

    public class RatingWrapper
    {
        [JsonProperty("value")]
        public decimal Value { get; set; }
    }
}

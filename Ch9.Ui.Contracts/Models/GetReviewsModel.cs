using Newtonsoft.Json;

namespace Ch9.Ui.Contracts.Models
{
    public class GetReviewsModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("page")]
        public int Page { get; set; }

        [JsonProperty("results")]
        public Review[] Reviews { get; set; }

        [JsonProperty("total_pages")]
        public int TotalPages { get; set; }

        [JsonProperty("total_results")]
        public int TotalResults { get; set; }
    }

    public class Review
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
}

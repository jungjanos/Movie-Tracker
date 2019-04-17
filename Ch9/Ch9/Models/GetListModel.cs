using Newtonsoft.Json;

namespace Ch9.Models
{    
    public class GetListsModel
    {
        [JsonProperty("page")]
        public int Page { get; set; }

        [JsonProperty("results")]
        public MovieListModel[] MovieLists { get; set; }

        [JsonProperty("total_pages")]
        public int TotalPages { get; set; }

        [JsonProperty("total_results")]
        public int TotalResults { get; set; }
    }

    public class MovieListModel
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("favorite_count")]
        public int FavoriteCount { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("item_count")]
        public int ItemCount { get; set; }

        [JsonProperty("iso_639_1")]
        public string Iso639 { get; set; }

        [JsonProperty("list_type")]
        public string ListType { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("poster_path")]
        public string PosterPath { get; set; }
    }

    public class ListCrudResponseModel
    {
        [JsonProperty("status_message")]
        public string StatusMessage { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("status_code")]
        public int StatusCode { get; set; }

        [JsonProperty("list_id")]
        public int ListId { get; set; }
    }
}

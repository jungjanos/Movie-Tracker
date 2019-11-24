using Newtonsoft.Json;

namespace Ch9.Ui.Contracts.Models
{
    public class AccountDetailsModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("iso_639_1")]
        public string Iso639 { get; set; }
        [JsonProperty("iso_3166_1")]
        public string Iso3166 { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("include_adult")]
        public bool IncludeAdult { get; set; }
        [JsonProperty("username")]
        public string AccountName { get; set; }
    }
}

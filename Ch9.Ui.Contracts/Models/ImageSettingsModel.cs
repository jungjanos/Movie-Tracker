using Newtonsoft.Json;

namespace Ch9.Ui.Contracts.Models
{
    public class ImageSettingsModel
    {

        [JsonProperty("base_url")]
        public string BaseUrl { get; set; }

        [JsonProperty("secure_base_url")]
        public string SecureBaseUrl { get; set; }

        [JsonProperty("backdrop_sizes")]
        public string[] BackdropSizes { get; set; }

        [JsonProperty("logo_sizes")]
        public string[] LogoSizes { get; set; }

        [JsonProperty("poster_sizes")]
        public string[] PosterSizes { get; set; }

        [JsonProperty("profile_sizes")]
        public string[] ProfileSizes { get; set; }

        [JsonProperty("still_sizes")]
        public string[] StillSizes { get; set; }
    }
}

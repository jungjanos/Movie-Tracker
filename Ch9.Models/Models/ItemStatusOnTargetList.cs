using Newtonsoft.Json;

namespace Ch9.Models
{
    public class ItemStatusOnTargetList
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("item_present")]
        public bool ItemPresent { get; set; }
    }
}

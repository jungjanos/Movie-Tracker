using Newtonsoft.Json;

namespace Ch9.Ui.Contracts.Models
{
    public class GenreIdNamePairWrapper
    {
        public GenreIdNamePairWrapper() { }

        [JsonProperty("genres")]
        public GenreIdNamePair[] Genres { get; set; }
    }

    public class GenreIdNamePair
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}

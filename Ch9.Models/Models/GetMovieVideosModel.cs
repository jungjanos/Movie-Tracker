using Newtonsoft.Json;
using System.Collections.Generic;

namespace Ch9.Models
{
    public class GetMovieVideosModel
    {
        [JsonProperty("id")]
        public int MovieId { get; set; }

        [JsonProperty("results")]
        public List<TmdbVideoModel> VideoModels { get; set; }
    }
}

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Ch9.Ui.Contracts.Models
{
    public class GetMovieVideosModel
    {
        [JsonProperty("id")]
        public int MovieId { get; set; }

        [JsonProperty("results")]
        public List<TmdbVideoModel> VideoModels { get; set; }
    }
}

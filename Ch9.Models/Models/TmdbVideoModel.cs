using Newtonsoft.Json;
using System;

namespace Ch9.Models
{
    // Describes different kinds of movie videos on the TMDB ApiServer
    public class TmdbVideoModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("iso_639_1")]
        public string Iso { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("name")]
        public string Title { get; set; }

        [JsonProperty("site")]
        public string Site { get; set; }

        [JsonProperty("size")]
        public int Size { get; set; } // allowed: 360, 480, 720, 1080

        [JsonProperty("type")]
        public string TypeStr { get; set; } // allowed: Trailer, Teaser, Clip, Featurette, Behind the Scenes, Bloopers        

        public VideoStreamInfoSet Streams { get; set; }
    }

    [Flags]
    public enum VideoType
    {
        None = 0,
        Unspecified = 1,
        Trailer = 2,
        Teaser = 4,
        Clip = 8,
        Featurette = 16,
        BehindTheScene = 32,
        Blooper = 64,
        Other = 128
    }
}

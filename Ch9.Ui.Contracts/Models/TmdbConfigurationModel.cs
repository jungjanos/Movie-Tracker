using Newtonsoft.Json;

namespace Ch9.Ui.Contracts.Models
{
    public class TmdbConfigurationModel
    {
        [JsonProperty("images")]
        public ImageSettingsModel Images { get; set; }

        [JsonProperty("change_keys")]
        public string[] ChangeKeys { get; set; }

        // Includes the defaults as of 2019 in case network problems
        // prevent fetching actual state from the server
        public static TmdbConfigurationModel StaticDefaults;

        static TmdbConfigurationModel()
        {
            StaticDefaults = new TmdbConfigurationModel
            {
                Images = new ImageSettingsModel
                {
                    BaseUrl = @"http://image.tmdb.org/t/p/",
                    SecureBaseUrl = @"https://image.tmdb.org/t/p/",

                    BackdropSizes = new string[]
                     {
                        "w300",
                        "w780",
                        "w1280",
                        "original"
                     },
                    LogoSizes = new string[]
                     {
                          "w45",
                          "w92",
                          "w154",
                          "w185",
                          "w300",
                          "w500",
                          "original"
                     },
                    PosterSizes = new string[]
                     {
                          "w92",
                          "w154",
                          "w185",
                          "w342",
                          "w500",
                          "w780",
                          "original"
                     },
                    ProfileSizes = new string[]
                     {
                        "w45",
                        "w185",
                        "h632",
                        "original"
                     },
                    StillSizes = new string[]
                     {
                        "w92",
                        "w185",
                        "w300",
                        "original"
                     }
                },
                ChangeKeys = new string[]
                 {
                    "adult",
                    "air_date",
                    "also_known_as",
                    "alternative_titles",
                    "biography",
                    "birthday",
                    "budget",
                    "cast",
                    "certifications",
                    "character_names",
                    "created_by",
                    "crew",
                    "deathday",
                    "episode",
                    "episode_number",
                    "episode_run_time",
                    "freebase_id",
                    "freebase_mid",
                    "general",
                    "genres",
                    "guest_stars",
                    "homepage",
                    "images",
                    "imdb_id",
                    "languages",
                    "name",
                    "network",
                    "origin_country",
                    "original_name",
                    "original_title",
                    "overview",
                    "parts",
                    "place_of_birth",
                    "plot_keywords",
                    "production_code",
                    "production_companies",
                    "production_countries",
                    "releases",
                    "revenue",
                    "runtime",
                    "season",
                    "season_number",
                    "season_regular",
                    "spoken_languages",
                    "status",
                    "tagline",
                    "title",
                    "translations",
                    "tvdb_id",
                    "tvrage_id",
                    "type",
                    "video",
                    "videos"
                 }
            };
        }
    }
}

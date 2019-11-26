using Newtonsoft.Json;
using System;

namespace Ch9.Ui.Contracts.Models
{
    public class PersonsDetailsModel
    {
        [JsonProperty("birthday")]
        public string Birthday { get; set; }

        public int? Age
        {
            get
            {
                if (DateTime.TryParse(Birthday, out DateTime birthDay))
                {
                    int years = DateTime.Now.Year - birthDay.Year;
                    bool bdayStillComming = DateTime.Now.DayOfYear < birthDay.DayOfYear;

                    return bdayStillComming ? years - 1 : years;
                }
                else
                    return null;
            }
        }
        [JsonProperty("known_for_department")]
        public string KnownFor { get; set; }

        [JsonProperty("deathday")]
        public string DeathDay { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("also_known_as")]
        public string[] AlsoKnownAs { get; set; }

        [JsonProperty("gender")]
        public int Sex { get; set; }

        [JsonProperty("biography")]
        public string Biography { get; set; }

        [JsonProperty("popularity")]
        public decimal Popularity { get; set; }

        [JsonProperty("place_of_birth")]
        public string PlaceOfBirth { get; set; }

        [JsonProperty("profile_path")]
        public string ProfilePath { get; set; }

        [JsonProperty("adult")]
        public bool Adult { get; set; }

        [JsonProperty("imdb_id")]
        public string ImdbId { get; set; }

        [JsonProperty("homepage")]
        public string Homepage { get; set; }
    }
}

using Newtonsoft.Json;

namespace Ch9.Models
{
    public class GetPersonsMovieCreditsModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("cast")]
        ActorsMovieCredit[] MoviesAsActor { get; set; }

        [JsonProperty("crew")]
        CrewMembersMovieCredit[] MoviesAsCrewMember { get; set; }
    }

    public class ActorsMovieCredit : MovieDetailModel
    {
        [JsonProperty("character")]
        public string Character { get; set; }

        [JsonProperty("credit_id")]
        public string CreditId { get; set; }
    }
    public class CrewMembersMovieCredit : MovieDetailModel
    {
        [JsonProperty("department")]
        public string Department { get; set; }

        [JsonProperty("job")]
        public string Job { get; set; }
        [JsonProperty("credit_id")]
        public string CreditId { get; set; }
    }

}

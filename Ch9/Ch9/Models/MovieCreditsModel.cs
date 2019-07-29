﻿using Newtonsoft.Json;

namespace Ch9.Models
{
    public class MovieCreditsModel
    {
        [JsonProperty("id")]
        public int MovieId { get; set; }

        [JsonProperty("cast")]
        public MovieCastModel[] Cast { get; set; }

        [JsonProperty("crew")]
        public MovieCrewModel[] Crew { get; set; }
    }

    /// <summary>
    /// interface to display diferent types of staff (crew and actors) on the UI     
    /// </summary>
    public interface IStaffMember
    {        
        string Name { get; }
        string Role { get; }
        string ProfilePath { get; set; }
        int Id { get; }
        string CreditId { get; }
    }

    public class MovieCastModel : IStaffMember
    {
        [JsonProperty("cast_id")]
        public int CastId { get; set; }

        [JsonProperty("character")]
        public string Character { get; set; }

        [JsonProperty("credit_id")]
        public string CreditId { get; set; }

        [JsonProperty("gender")]
        public int? Gender { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("order")]
        public int Order { get; set; }

        [JsonProperty("profile_path")]
        public string ProfilePath { get; set; }

        public string Role => Character;
    }

    public class MovieCrewModel : IStaffMember
    {
        [JsonProperty("credit_id")]
        public string CreditId { get; set; }

        [JsonProperty("department")]
        public string Department { get; set; }

        [JsonProperty("gender")]
        public int? Gender { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("job")]
        public string Job { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("profile_path")]
        public string ProfilePath { get; set; }

        public string Role => Job;
    }
}

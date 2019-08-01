using Newtonsoft.Json;

namespace Ch9.Models
{
    public class RequestToken
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        // To use this field would require a custom DateTime conversion        
        //[JsonProperty("expires_at")]
        //public DateTime ExpiresAt { get; set; }

        [JsonProperty("request_token")]
        public string Token { get; set; }

    }
}

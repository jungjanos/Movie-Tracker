using Newtonsoft.Json;

namespace Ch9.Authentication
{
    public class SessionIdResponseModel
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("session_id")]
        public string SessionId { get; set; }
    }
}

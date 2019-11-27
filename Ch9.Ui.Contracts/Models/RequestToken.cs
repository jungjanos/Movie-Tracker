﻿using Newtonsoft.Json;

namespace Ch9.Ui.Contracts.Models
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
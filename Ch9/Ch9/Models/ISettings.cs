﻿namespace Ch9.Models
{
    public interface ISettings
    {
        string AccountName { get; set; }
        string ApiKey { get; set; }
        bool HasTmdbAccount { get; set; }
        bool IncludeAdult { get; set; }
        string Password { get; set; }
        string SearchLanguage { get; set; }
        int SearchPeriod { get; set; }
        string SessionId { get; set; }
    }
}
using System.Collections.Generic;
using Xamarin.Forms;

namespace Ch9.Models
{
    // Settings uses Application.Properties dictionary with data binding to UI
    // Default values are currently hard coded
    public class Settings
    {
        private IDictionary<string, object> appDictionary;

        public Settings()
        {
            appDictionary = Application.Current.Properties;
        }       

        public string ApiKey
        {
            get
            {
                if (appDictionary.ContainsKey(nameof(ApiKey)))
                    return appDictionary[nameof(ApiKey)].ToString();
                else
                    return "764d596e888359d26c0dc49deffecbb3";
            }
            set => appDictionary[nameof(ApiKey)] = value;
        }

        public bool HasTmdbAccount
        {
            get
            {
                if (appDictionary.ContainsKey(nameof(HasTmdbAccount)))
                    return (bool)appDictionary[nameof(HasTmdbAccount)];
                else
                    return false;
            }
            set => appDictionary[nameof(HasTmdbAccount)] = value;
        }

        public string SessionId
        {
            get
            {
                if (appDictionary.ContainsKey(nameof(SessionId)))
                    return appDictionary[nameof(SessionId)].ToString();
                else
                    return null;
            }
            set => appDictionary[nameof(SessionId)] = value;
        }

        public string AccountName
        {
            get
            {
                if (appDictionary.ContainsKey(nameof(AccountName)))
                    return appDictionary[nameof(AccountName)].ToString();
                else
                    return null;
            }
            set => appDictionary[nameof(AccountName)] = value;
        }

        public string Password
        {
            get
            {
                if (appDictionary.ContainsKey(nameof(Password)))
                    return appDictionary[nameof(Password)].ToString();
                else
                    return null;
            }
            set => appDictionary[nameof(Password)] = value;
        }

        public bool IncludeAdult
        {
            get
            {
                if (appDictionary.ContainsKey(nameof(IncludeAdult)))
                    return (bool)appDictionary[nameof(IncludeAdult)];
                else
                    return false;
            }
            set => appDictionary[nameof(IncludeAdult)] = value;
        }

        public int SearchPeriod
        {
            get
            {
                if (appDictionary.ContainsKey(nameof(SearchPeriod)))
                    return (int)appDictionary[nameof(SearchPeriod)];
                else
                    return 25;
            }
            set => appDictionary[nameof(SearchPeriod)] = value;
        }
        
        public string SearchLanguage
        {
            get
            {
                if (appDictionary.ContainsKey(nameof(SearchLanguage)))
                    return appDictionary[nameof(SearchLanguage)].ToString();
                else
                    return "en";
            }
            set => appDictionary[nameof(SearchLanguage)] = value;
        }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;
using System.Linq;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Collections.Specialized;

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

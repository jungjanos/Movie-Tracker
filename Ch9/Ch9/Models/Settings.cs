using System.Collections.Generic;
using Xamarin.Forms;

namespace Ch9.Models
{
    // Settings uses Application.Properties dictionary with data binding to UI
    // Default values are currently hard coded
    public class Settings : ISettings
    {
        private readonly IDictionary<string, object> _appDictionary;

        public Settings(IDictionary<string, object> settingsDictionary = null)
        {            
            _appDictionary =  settingsDictionary ?? Application.Current.Properties;
        }

        public string ApiKey
        {
            get
            {
                if (_appDictionary.ContainsKey(nameof(ApiKey)))
                    return _appDictionary[nameof(ApiKey)].ToString();
                else
                    return "764d596e888359d26c0dc49deffecbb3";
            }
            set => _appDictionary[nameof(ApiKey)] = value;
        }

        public bool HasTmdbAccount
        {
            get
            {
                if (_appDictionary.ContainsKey(nameof(HasTmdbAccount)))
                    return (bool)_appDictionary[nameof(HasTmdbAccount)];
                else
                    return false;
            }
            set => _appDictionary[nameof(HasTmdbAccount)] = value;
        }

        public string SessionId
        {
            get
            {
                if (_appDictionary.ContainsKey(nameof(SessionId)))
                    return _appDictionary[nameof(SessionId)].ToString();
                else
                    return null;
            }
            set => _appDictionary[nameof(SessionId)] = value;
        }

        public string AccountName
        {
            get
            {
                if (_appDictionary.ContainsKey(nameof(AccountName)))
                    return _appDictionary[nameof(AccountName)].ToString();
                else
                    return null;
            }
            set => _appDictionary[nameof(AccountName)] = value;
        }

        public string Password
        {
            get
            {
                if (_appDictionary.ContainsKey(nameof(Password)))
                    return _appDictionary[nameof(Password)].ToString();
                else
                    return null;
            }
            set => _appDictionary[nameof(Password)] = value;
        }

        public bool IncludeAdult
        {
            get
            {
                if (_appDictionary.ContainsKey(nameof(IncludeAdult)))
                    return (bool)_appDictionary[nameof(IncludeAdult)];
                else
                    return false;
            }
            set => _appDictionary[nameof(IncludeAdult)] = value;
        }

        public int SearchPeriod
        {
            get
            {
                if (_appDictionary.ContainsKey(nameof(SearchPeriod)))
                    return (int)_appDictionary[nameof(SearchPeriod)];
                else
                    return 25;
            }
            set => _appDictionary[nameof(SearchPeriod)] = value;
        }

        public string SearchLanguage
        {
            get
            {
                if (_appDictionary.ContainsKey(nameof(SearchLanguage)))
                    return _appDictionary[nameof(SearchLanguage)].ToString();
                else
                    return "en";
            }
            set => _appDictionary[nameof(SearchLanguage)] = value;
        }


        // This item will hold the MovieList id which has been selected as the primary MovieList
        // Where all the single-click Movie add operations will place the user added movies.
        public int? ActiveMovieListId
        {
            get
            {
                if (_appDictionary.ContainsKey(nameof(ActiveMovieListId)))
                    return (int?)_appDictionary[nameof(ActiveMovieListId)];
                else
                    return null;
            }
            set => _appDictionary[nameof(ActiveMovieListId)] = value;
        }

    }
}

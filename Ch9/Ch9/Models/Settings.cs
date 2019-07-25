using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            _appDictionary = settingsDictionary ?? Application.Current.Properties;
        }

        public string ApiKey
        {
            get
            {
                if (_appDictionary.ContainsKey(nameof(ApiKey)))
                    return _appDictionary[nameof(ApiKey)] as string;
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
                    return _appDictionary[nameof(SessionId)] as string;
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
                    return _appDictionary[nameof(AccountName)] as string;
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
                    return _appDictionary[nameof(Password)] as string;
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
                    return _appDictionary[nameof(SearchLanguage)] as string;
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

        // This item holds the Id-s of the Movies on the active MovieList
        // returns null if there is no active MovieList
        // returns empty if the active MovieList does not contain movies
        public int[] MovieIdsOnActiveList
        {
            get
            {
                if (_appDictionary.ContainsKey(nameof(MovieIdsOnActiveList)))
                {
                    var json = (string)_appDictionary[nameof(MovieIdsOnActiveList)];
                    return JsonConvert.DeserializeObject<int[]>(json);
                }
                else
                    return null;
            }
            set => _appDictionary[nameof(MovieIdsOnActiveList)] = JsonConvert.SerializeObject(value);
        }
        
        public string PlaybackQualityStr
        {
            get
            {
                if (_appDictionary.ContainsKey(nameof(PlaybackQualityStr)))
                    return _appDictionary[nameof(PlaybackQualityStr)] as string;
                else
                    return (VideoPlaybackQuality.Low).ToString();
            }
            set => _appDictionary[nameof(PlaybackQualityStr)] = value;
        }

        public VideoPlaybackQuality PlaybackQuality
        {
            get
            {
                if (PlaybackQualityStr == VideoPlaybackQuality.Low.ToString())
                    return VideoPlaybackQuality.Low;
                else if (PlaybackQualityStr == VideoPlaybackQuality.High.ToString())
                    return VideoPlaybackQuality.High;

                else return VideoPlaybackQuality.Low;
            }
        }
            

        public async Task SavePropertiesAsync()
        {
            await Application.Current.SavePropertiesAsync();
        }
    }
}

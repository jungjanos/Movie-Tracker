using Ch9.Models;
using Ch9.Services.VideoService;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Ch9.Services
{
    // Settings uses Application.Properties dictionary with data binding to UI
    // Default values are currently hard coded
    public partial class Settings : ISettings, INotifyPropertyChanged
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

        public bool IsLoggedin => !string.IsNullOrEmpty(SessionId);        

        public string SessionId
        {
            get
            {
                if (_appDictionary.ContainsKey(nameof(SessionId)))
                    return _appDictionary[nameof(SessionId)] as string;
                else
                    return null;
            }
            set
            {
                if ( SessionId != value)
                {
                    _appDictionary[nameof(SessionId)] = value;
                    OnPropertyChanged(nameof(SessionId));
                    OnPropertyChanged(nameof(IsLoggedin));
                }
            }
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
            set
            {
                if (AccountName != value)
                {
                    _appDictionary[nameof(AccountName)] = value;
                    OnPropertyChanged(nameof(AccountName));
                }
            }
                
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

        public bool SafeSearch
        {
            get
            {
                if (_appDictionary.ContainsKey(nameof(SafeSearch)))
                    return (bool)_appDictionary[nameof(SafeSearch)];
                else
                    return true;
            }
            set => _appDictionary[nameof(SafeSearch)] = value;
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

        /// <summary>
        /// Xamarin forms does not allow to bind Enum source to Picker, only string is allowed
        /// this translates between Picker, Application Dictionary and InformationLinkTargetHomePage enum
        /// </summary>
        public string InformationLinksTargetHomePageStr
        {
            get
            {
                if (_appDictionary.ContainsKey(nameof(InformationLinksTargetHomePageStr)))
                    return _appDictionary[nameof(InformationLinksTargetHomePageStr)] as string;
                else
                    return InformationLinkTargetHomePage.IMDb.ToString();
            }
            set => _appDictionary[nameof(InformationLinksTargetHomePageStr)] = value;
        }

        public InformationLinkTargetHomePage InfoLinkTargetHomePage
        {
            get
            {
                if (InformationLinksTargetHomePageStr == InformationLinkTargetHomePage.Invalid.ToString())
                    return InformationLinkTargetHomePage.Invalid;
                else if (InformationLinksTargetHomePageStr == InformationLinkTargetHomePage.IMDb.ToString())
                    return InformationLinkTargetHomePage.IMDb;
                else if (InformationLinksTargetHomePageStr == InformationLinkTargetHomePage.TMDb.ToString())
                    return InformationLinkTargetHomePage.TMDb;

                else return InformationLinkTargetHomePage.Invalid;
            }
        }

        /// <summary>
        /// Xamarin forms does not allow to bind Enum source to Picker, only string is allowed
        /// this translates between Picker, Application Dictionary and VideoPlaybackQuality enum
        /// </summary>
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

        public bool UseHttpsForImages
        {
            get
            {
                if (_appDictionary.ContainsKey(nameof(UseHttpsForImages)))
                    return (bool)_appDictionary[nameof(UseHttpsForImages)];
                else return true;
            }
            set => _appDictionary[nameof(UseHttpsForImages)] = value;
        }

        public bool IsLoginPageDeactivationRequested
        {
            get
            {
                if (_appDictionary.ContainsKey(nameof(IsLoginPageDeactivationRequested)))
                    return (bool)_appDictionary[nameof(IsLoginPageDeactivationRequested)];
                else return false;
            }
            set => _appDictionary[nameof(IsLoginPageDeactivationRequested)] = value;
        }

        public async Task SavePropertiesAsync() =>
            await Application.Current.SavePropertiesAsync();

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName]string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}

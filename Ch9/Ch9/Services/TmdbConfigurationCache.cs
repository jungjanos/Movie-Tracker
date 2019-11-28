using Ch9.Ui.Contracts.Models;
using Ch9.Utils;
using Ch9.Services.Contracts;
using Ch9.Data.Contracts;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ch9.Services
{
    // tries to get the TMDB configuration from the server, caches the result in the property dictionary
    // if contacting the server fails, it consults the property dictionary
    // if the property dictionary contains no entry, the static defaults are returned 
    public interface ITmdbConfigurationCache
    {
        Task<TmdbConfigurationModel> FetchTmdbConfiguration(int retries = 0, int retryDelay = 1000);
    }

    public class TmdbConfigurationCache : ITmdbConfigurationCache
    {        
        private readonly ITmdbApiService _tmdbApiService;
        private readonly IPersistLocalSettings _localSettingsPersister;
        private readonly IDictionary<string, object> _appDictionary;        

        private TmdbConfigurationModel TmdbConfigurationModel
        {
            get
            {
                if (_appDictionary.ContainsKey(nameof(TmdbConfigurationModel)))
                {
                    var json = (string)_appDictionary[nameof(TmdbConfigurationModel)];
                    return JsonConvert.DeserializeObject<TmdbConfigurationModel>(json);
                }
                else return null;
            }
            set => _appDictionary[nameof(TmdbConfigurationModel)] = JsonConvert.SerializeObject(value);
        }

        public TmdbConfigurationCache(            
            ITmdbApiService tmdbApiService,
            IPersistLocalSettings localSettingsPersister = null,
            IDictionary<string, object> appDictionary = null
            )
        {            
            _tmdbApiService = tmdbApiService;
            _localSettingsPersister = localSettingsPersister;
            _appDictionary = appDictionary;
        }

        public async Task<TmdbConfigurationModel> FetchTmdbConfiguration(int retries = 0, int retryDelay = 1000)
        {
            TmdbConfigurationModel result = null;

            try
            {
                var response = await _tmdbApiService.TryGetTmdbConfiguration(retries, retryDelay, fromCache: true);

                if (response.HttpStatusCode.IsSuccessCode())
                {
                    result = TmdbConfigurationModel = response.ConfigurationModel;
                    await _localSettingsPersister?.SavePropertiesAsync();
                }
                else
                    result = TmdbConfigurationModel ?? TmdbConfigurationModel.StaticDefaults;
            }
            catch { };

            return result;
        }
    }
}

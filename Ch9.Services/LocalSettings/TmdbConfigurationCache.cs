using Ch9.Ui.Contracts.Models;
using Ch9.Infrastructure.Extensions;
using Ch9.Services.Contracts;
using Ch9.Data.Contracts;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ch9.Services
{
    public class TmdbConfigurationCache : ITmdbConfigurationCache
    {        
        private readonly ITmdbApiService _tmdbApiService;
        private readonly IPersistLocalSettings _localSettingsPersister;
        private readonly IDictionary<string, object> _appDictionary;        

        public TmdbConfigurationModel TmdbConfigurationModel
        {
            get
            {
                if (_appDictionary.ContainsKey(nameof(TmdbConfigurationModel)))
                {
                    var json = (string)_appDictionary[nameof(TmdbConfigurationModel)];
                    return JsonConvert.DeserializeObject<TmdbConfigurationModel>(json);
                }
                else return TmdbConfigurationModel.Defaults;
            }
            private set => _appDictionary[nameof(TmdbConfigurationModel)] = JsonConvert.SerializeObject(value);
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
        /// <summary>
        /// tries to get the TMDB configuration from the server, caches the result in the property dictionary
        //  if contacting the server fails, it consults the property dictionary        
        /// </summary>
        public async Task FetchAndPersistTmdbConfiguration(int retries = 0, int retryDelay = 1000)
        {           
            try
            {
                var response = await _tmdbApiService.TryGetTmdbConfiguration(retries, retryDelay, fromCache: true);

                if (response.HttpStatusCode.IsSuccessCode())
                {
                    TmdbConfigurationModel = response.ConfigurationModel;
                    await _localSettingsPersister?.SavePropertiesAsync();
                }
            }
            catch { };

            return;
        }
    }
}

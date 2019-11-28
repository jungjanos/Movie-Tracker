using Ch9.Ui.Contracts.Models;
using Ch9.Infrastructure.Extensions;
using Ch9.Services.Contracts;
using Ch9.Data.Contracts;
using System.Threading.Tasks;

namespace Ch9.Services
{
    public class TmdbConfigurationCache : ITmdbConfigurationCache
    {        
        private readonly ITmdbApiService _tmdbApiService;
        private readonly IPersistLocalSettings _localSettingsPersister;
        private readonly ISettings _settings;

        public TmdbConfigurationModel TmdbConfigurationModel
        {
            get => _settings.TmdbConfigurationModel;            
        }

        public TmdbConfigurationCache(
            ITmdbApiService tmdbApiService,
            ISettings settings,
            IPersistLocalSettings localSettingsPersister = null            
            )
        {            
            _tmdbApiService = tmdbApiService;
            _settings = settings;
            _localSettingsPersister = localSettingsPersister;            
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
                    _settings.TmdbConfigurationModel = response.ConfigurationModel;
                    await _settings.SavePropertiesAsync();
                }
            }
            catch { };

            return;
        }
    }
}

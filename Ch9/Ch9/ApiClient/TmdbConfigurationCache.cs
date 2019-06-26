﻿using Ch9.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Ch9.ApiClient
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
        private readonly ITmdbCachedSearchClient _tmdbCachedSearchClient;
        private readonly IDictionary<string, object> _appDictionary;
        private readonly Application _xamarinApplication;

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
            ITmdbCachedSearchClient tmdbCachedSearchClient,
            IDictionary<string, object> appDictionary = null, 
            Application xamarinApplication = null
            )
        {
            _tmdbCachedSearchClient = tmdbCachedSearchClient;
            _appDictionary = appDictionary ?? Application.Current.Properties;
            _xamarinApplication = xamarinApplication;
        }

        public async Task<TmdbConfigurationModel> FetchTmdbConfiguration(int retries = 0, int retryDelay = 1000)
        {
            TmdbConfigurationModel result = null;

            try
            {
                var response = await _tmdbCachedSearchClient.GetTmdbConfiguration(retries, retryDelay, fromCache: true);

                if (response.HttpStatusCode.IsSuccessCode())
                {
                    result = JsonConvert.DeserializeObject<TmdbConfigurationModel>(response.Json);
                    TmdbConfigurationModel = result;
                    await _xamarinApplication?.SavePropertiesAsync();
                }
                else
                    result = TmdbConfigurationModel ?? TmdbConfigurationModel.StaticDefaults;
            }
            catch { };

            return result;
        }
    }
}

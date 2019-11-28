using Ch9.Ui.Contracts.Models;
using System.Threading.Tasks;

namespace Ch9.Services.Contracts
{
    // tries to get the TMDB configuration from the server, caches the result in the property dictionary
    // if contacting the server fails, it consults the property dictionary
    // if the property dictionary contains no entry, the static defaults are returned 
    public interface ITmdbConfigurationCache
    {
        Task<TmdbConfigurationModel> FetchTmdbConfiguration(int retries = 0, int retryDelay = 1000);
    }
}

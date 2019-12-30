using Ch9.Models;

using System.Threading.Tasks;

namespace Ch9.Services.Contracts
{
    public interface ITmdbConfigurationCache
    {
        TmdbConfigurationModel TmdbConfigurationModel { get; }

        Task FetchAndPersistTmdbConfiguration(int retries = 0, int retryDelay = 1000);
    }
}

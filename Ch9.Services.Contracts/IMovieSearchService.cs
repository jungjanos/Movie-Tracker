using Ch9.Models;

using System.Threading.Tasks;

namespace Ch9.Services.Contracts
{
    public interface IMovieSearchService
    {
        Task<SearchResult> LoadResultPage(string searchString, int pageToLoad, int retryCount, int delayMilliseconds, bool fromCache);
    }
}
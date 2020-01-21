using Ch9.Models;

using System.Threading.Tasks;

namespace Ch9.Services.Contracts
{
    public interface ITrendingMoviesService
    {
        Task<SearchResult> LoadTrendingPage(bool week, int page, int retryCount, int delayMilliseconds);
    }
}
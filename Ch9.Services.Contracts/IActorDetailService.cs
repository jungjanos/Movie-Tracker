using Ch9.Models;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ch9.Services.Contracts
{
    public interface IActorDetailService
    {
        Task<IList<ImageModel>> LoadPersonsImageCollection(int personId, int retryCount, int delayMilliseconds, bool fromCahe = true);
        Task<PersonsMovieCreditsModel> LoadPersonsMovieCredits(int personId, int retryCount, int delayMilliseconds, bool fromCahe = true);
    }
}
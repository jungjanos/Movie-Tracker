using Ch9.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ch9.Services.VideoService
{
    public interface IVideoService
    {
        Task<List<ImageModel>> GetVideoThumbnails(int movieId, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true);
        Task PopulateWithStreams(TmdbVideoModel attachedVideo);
    }
}

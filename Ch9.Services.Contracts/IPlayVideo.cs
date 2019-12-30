using System.Threading.Tasks;

namespace Ch9.Services.Contracts
{
    public interface IPlayVideo
    {
        /// <summary>
        /// Opens a Youtube video via its Url in the platforms default video player.
        /// This is the legally safe option
        /// </summary>
        /// <param name="youtubeUrl">Url to a Youtube video</param>
        Task OpenYoutubeVideoInDeviceDefaultPlayer(string youtubeUrl);

        /// <summary>
        /// Opens a media stream via direct stream-Url using the applications built in video/stream player
        /// </summary>
        /// <param name="streamUrl"></param>
        Task OpenVideoStreamDirectly(string streamUrl);
    }
}

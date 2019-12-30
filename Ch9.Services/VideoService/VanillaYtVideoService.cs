using Ch9.Services.Contracts;
using Ch9.Ui.Contracts.Models;

using System.Threading.Tasks;

namespace Ch9.Services.VideoService
{
    /// <summary>
    /// This class provides video service in line with Google's requirements.
    /// Opening videos based on Url on the device specific default player.
    /// 
    /// Hogy google barátunk valmelyik robotja ne auto-termináljon minket...
    /// </summary>
    public class VanillaYtVideoService : VideoServiceBase
    {
        private string GetVideoUrlFromId(string id) => $"http://www.youtube.com/watch?v={id}";

        public VanillaYtVideoService(
            ISettings settings,
            ITmdbApiService tmdbApiService,
            IPlayVideo videoPlayer) :
            base(settings, tmdbApiService, videoPlayer)
        { }

        public override async Task PlayVideo(TmdbVideoModel attachedVideo) =>
        await _videoPlayer.OpenYoutubeVideoInDeviceDefaultPlayer(GetVideoUrlFromId(attachedVideo.Key));
    }
}

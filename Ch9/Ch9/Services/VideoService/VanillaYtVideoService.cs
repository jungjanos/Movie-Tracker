﻿using System.Threading.Tasks;
using Ch9.ApiClient;
using Ch9.Models;

namespace Ch9.Services.VideoService
{
    /// <summary>
    /// This class provides video service in line with Google's requirements.
    /// Opening videos based on Url on the device specific default player.
    /// 
    /// Hogy google barátunk valmelyik robotja ne auto-termináljon minket...
    /// </summary>
    public class VanillaYtVideoService : VideoServiceBase, IVideoService
    {
        private string GetVideoUrlFromId(string id) => $"http://www.youtube.com/watch?v={id}";

        public VanillaYtVideoService(
            ISettings settings,
            ITmdbCachedSearchClient tmdbCachedSearchClient) : 
            base(settings, tmdbCachedSearchClient)
        { }

        public async Task PlayVideo(TmdbVideoModel attachedVideo, IPageService pageService) =>
            await pageService.OpenWeblink(GetVideoUrlFromId(attachedVideo.Key));
    }
}

using Ch9.Ui.Contracts.Models;

using System.Collections.Generic;

namespace Ch9.Services.VideoService
{
    /// <summary>
    /// Selects a video stream based on user setting for desired quality level
    /// If no stream fulfills the selection criteria, "null" must be returned
    /// </summary>
    public interface IVideoStreamSelector
    {
        VideoStreamInfo SelectVideoStream(IEnumerable<VideoStreamInfo> streams);
    }
}

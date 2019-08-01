using System;
using System.Collections.Generic;

namespace Ch9.Services.VideoService
{
    /// <summary>
    /// Contains all url video stream sources for a single video. Selects the stream best fitting user's 
    /// requirements based on the stream selector passed to its constructor
    /// </summary>
    public class VideoStreamInfoSet
    {
        private readonly IEnumerable<VideoStreamInfo> _videoStreams;
        private readonly IVideoStreamSelector _videoStreamSelector;
        /// <summary>
        /// The stream sources become invalid after the specified point in time. 
        /// Has to be checked by caller.  
        /// </summary>
        public DateTimeOffset ValidUntil { get; }

        public VideoStreamInfoSet(IEnumerable<VideoStreamInfo> videoStreams, DateTimeOffset validUntil, IVideoStreamSelector videoStreamSelector)
        {
            _videoStreams = videoStreams;
            ValidUntil = validUntil;
            _videoStreamSelector = videoStreamSelector;
        }

        public VideoStreamInfo SelectedVideoStream =>
            _videoStreamSelector.SelectVideoStream(_videoStreams);

        public IEnumerable<VideoStreamInfo> VideoStreams => _videoStreams;
    }
}

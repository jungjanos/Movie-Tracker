using System;
using System.Collections.Generic;

namespace Ch9.Services.VideoService
{
    /// <summary>
    /// Contains all url video stream sources for a single video. 
    /// </summary>
    public class VideoStreamInfoSet
    {
        /// <summary>
        /// The stream sources become invalid after the specified point in time. 
        /// Has to be checked by caller.  
        /// </summary>
        public DateTimeOffset ValidUntil { get; }

        public VideoStreamInfoSet(IEnumerable<VideoStreamInfo> videoStreams, DateTimeOffset validUntil)
        {
            VideoStreams = videoStreams;
            ValidUntil = validUntil;            
        }

        public IEnumerable<VideoStreamInfo> VideoStreams { get; }
    }
}

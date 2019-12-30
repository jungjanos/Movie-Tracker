using System;
using System.Collections.Generic;

namespace Ch9.Models
{
    /// <summary>
    /// Contains all url video stream sources for a single video. 
    /// </summary>
    public class VideoStreamInfoSet
    {
        public DateTimeOffset ValidUntil { get; }

        public VideoStreamInfoSet(IEnumerable<VideoStreamInfo> videoStreams, DateTimeOffset validUntil)
        {
            VideoStreams = videoStreams;
            ValidUntil = validUntil;
        }

        public IEnumerable<VideoStreamInfo> VideoStreams { get; }
    }
}

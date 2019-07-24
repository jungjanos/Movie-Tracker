using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ch9.Models
{
    public interface IVideoService
    {
        Task<List<ImageModel>> GetVideoThumbnails(int movieId, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true);

        ///// <summary>
        ///// deprecated, shouldnt be used: too slow, too much work, breakable
        ///// </summary>
        //Task<List<ImageModel>> GetVideoThumbnailsWithVideoStreams(int movieId, int retryCount = 0, int delayMilliseconds = 1000, bool fromCache = true);
    }

    /// <summary>
    /// Class containing important metadata about the video itself
    /// </summary>
    public class VideoInfo
    {
        public VideoInfo(             
            string author, 
            DateTimeOffset uploadDate, 
            string title, 
            string description, 
            TimeSpan duration,             
            Statistics statistics 
            )
        {            
            Author = author;
            UploadDate = uploadDate;
            Title = title;
            Description = description;
            Duration = duration;
            Statistics = statistics;
        }
        
        public string Author { get; }
        public DateTimeOffset UploadDate { get; }
        public string Title { get; }
        public string Description { get; }
        public TimeSpan Duration { get; }        
        public Statistics Statistics { get; }
    }


    /// <summary>
    /// User activity statistics for videos
    /// </summary>
    public class Statistics
    {
        public Statistics(long viewCount, long likeCount, long dislikeCount, double averageRating)
        {
            ViewCount = viewCount;
            LikeCount = likeCount;
            DislikeCount = dislikeCount;
            AverageRating = averageRating;
        }

        public long ViewCount { get; }
        public long LikeCount { get; }
        public long DislikeCount { get; }
        public double AverageRating { get; }
    }
    /// <summary>
    /// Selects a video stream based on user setting for desired quality level
    /// If no stream fulfills the selection criteria, "null" must be returned
    /// </summary>
    public interface IVideoStreamSelector
    {
        VideoStreamInfo SelectVideoStream(IEnumerable<VideoStreamInfo> streams);
    }

    /// <summary>
    /// Contains properties of a single url video stream source
    /// </summary>
    public class VideoStreamInfo 
    {
        public VideoStreamInfo(string streamUrl, VideoQuality quality, string qualityLabel, int height, int width)
        {
            StreamUrl = streamUrl;
            Quality = quality;
            QualityLabel = qualityLabel;
            Height = height;
            Width = width;
        }
        public string StreamUrl { get; }
        public VideoQuality Quality { get; }
        public string QualityLabel { get; }
        public int Height { get; }
        public int Width { get; }        
    }

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


    /// <summary>
    /// Quality tag for a video stream
    /// </summary>
    public enum VideoQuality
    {
        /// <summary>
        /// Low quality (144p)
        /// </summary>
        Low144 = 0,

        /// <summary>
        /// Low quality (240p)
        /// </summary>
        Low240 = 1,

        /// <summary>
        /// Medium quality (360p)
        /// </summary>
        Medium360 = 2,

        /// <summary>
        /// Medium quality (480p)
        /// </summary>
        Medium480 = 3,

        /// <summary>
        /// High quality (720p)
        /// </summary>
        High720 = 4,

        /// <summary>
        /// High quality (1080p)
        /// </summary>
        High1080 = 5,

        /// <summary>
        /// High quality (1440p)
        /// </summary>
        High1440 = 6,

        /// <summary>
        /// High quality (2160p)
        /// </summary>
        High2160 = 7,

        /// <summary>
        /// High quality (2880p)
        /// </summary>
        High2880 = 8,

        /// <summary>
        /// High quality (3072p)
        /// </summary>
        High3072 = 9,

        /// <summary>
        /// High quality (4320p)
        /// </summary>
        High4320 = 10,

        /// <summary>
        /// invalid value
        /// </summary>
        Invalid = int.MinValue
    }
    public enum VideoPlaybackQuality
    {
        LowQ = 0,
        HighQ = 1
    }
}

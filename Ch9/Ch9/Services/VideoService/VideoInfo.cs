using System;

namespace Ch9.Services.VideoService
{
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

        public string DurationStr => $"{(int)Duration.TotalMinutes}:{Duration.Seconds:00}";
        public Statistics Statistics { get; }
    }
}

namespace Ch9.Services.VideoService
{
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
}

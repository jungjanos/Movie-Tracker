namespace Ch9.Models
{
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
}

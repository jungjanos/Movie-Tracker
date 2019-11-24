namespace Ch9.Ui.Contracts
{
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
        Low = 0,
        High = 1
    }
}

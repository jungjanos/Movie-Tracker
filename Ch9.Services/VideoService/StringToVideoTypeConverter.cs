using Ch9.Models;

namespace Ch9.Services.VideoService
{
    class StringToVideoTypeConverter
    {
        public VideoType Convert(string typeStr)
        {
            if (string.IsNullOrEmpty(typeStr))
                return VideoType.Unspecified;
            else if (string.Compare(typeStr, "trailer", System.StringComparison.OrdinalIgnoreCase) == 0)
                return VideoType.Trailer;
            else if (string.Compare(typeStr, "teaser", System.StringComparison.OrdinalIgnoreCase) == 0)
                return VideoType.Teaser;
            else if (string.Compare(typeStr, "clip", System.StringComparison.OrdinalIgnoreCase) == 0)
                return VideoType.Clip;
            else if (string.Compare(typeStr, "featurette", System.StringComparison.OrdinalIgnoreCase) == 0)
                return VideoType.Featurette;
            else if (string.Compare(typeStr, "behind the scenes", System.StringComparison.OrdinalIgnoreCase) == 0)
                return VideoType.BehindTheScene;
            else if (string.Compare(typeStr, "bloopers", System.StringComparison.OrdinalIgnoreCase) == 0)
                return VideoType.Blooper;
            else return VideoType.Other;
        }
    }
}

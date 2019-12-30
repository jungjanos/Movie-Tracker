#if !GOOGLEPLAY
using Ch9.Services.Contracts;
using Ch9.Ui.Contracts;

using System.Collections.Generic;
using System.Linq;

namespace Ch9.Services.VideoService
{
    public class YtVideoStreamSelector : IVideoStreamSelector
    {
        private readonly ISettings _settings;

        public YtVideoStreamSelector(ISettings settings) => _settings = settings;

        public Ui.Contracts.Models.VideoStreamInfo SelectVideoStream(IEnumerable<Ui.Contracts.Models.VideoStreamInfo> streams)
        {
            Ui.Contracts.Models.VideoStreamInfo result = null;

            var orderedByQuality = streams.OrderByDescending(s => s.Quality);

            if (_settings.PlaybackQuality == VideoPlaybackQuality.High)
            {
                result = orderedByQuality.Where(s => s.Quality > Ui.Contracts.VideoQuality.Medium480).LastOrDefault();
                result = result ?? orderedByQuality.FirstOrDefault();
            }
            else if (_settings.PlaybackQuality == VideoPlaybackQuality.Low)
            {
                result = orderedByQuality.Where(s => s.Quality < Ui.Contracts.VideoQuality.High720).FirstOrDefault();
                result = result ?? orderedByQuality.LastOrDefault();
            }

            return result;
        }
    }
}
#endif
using Ch9.Services.VideoService;
using Ch9.Utils;
using System.Threading.Tasks;
using static Ch9.Models.Settings;

namespace Ch9.Models
{
    public interface ISettings
    {
        string AccountName { get; set; }
        string ApiKey { get; set; }
        bool IsLoggedin { get; }
        bool SafeSearch { get; set; }
        string Password { get; set; }
        string SearchLanguage { get; set; }
        int SearchPeriod { get; set; }
        string SessionId { get; set; }
        int? ActiveMovieListId { get; set; }
        int[] MovieIdsOnActiveList { get; set; }
        VideoPlaybackQuality PlaybackQuality { get; }
        bool UseHttpsForImages { get; set; }
        string PlaybackQualityStr { get; set; }
        string InformationLinksTargetHomePageStr { get; set; }
        InformationLinkTargetHomePage InfoLinkTargetHomePage { get; }
        bool IsLoginPageDeactivationRequested { get; set; }

        Task SavePropertiesAsync();
    }
}
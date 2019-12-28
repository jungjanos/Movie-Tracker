using Ch9.Ui.Contracts;
using Ch9.Ui.Contracts.Models;

using System.Threading.Tasks;

namespace Ch9.Services.Contracts
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
        VideoType PreferredVideoTypes { get; }
        TmdbConfigurationModel TmdbConfigurationModel { get; set; }

        Task SavePropertiesAsync();
    }
}
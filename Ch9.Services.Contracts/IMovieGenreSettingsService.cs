using Ch9.Ui.Contracts.Models;
using System.Threading.Tasks;

namespace Ch9.Services.Contracts
{
    public interface IMovieGenreSettingsService
    {
        MovieGenreSettingsModel GetGenreSetting();
        Task SaveGenreSelection(MovieGenreSettingsModel movieGenreSettings);
        Task UpdateGenreListLanguage(string newLanguage, MovieGenreSettingsModel movieGenreSettings);
    }
}
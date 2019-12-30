using Ch9.Services;
using Ch9.Services.Contracts;
using Ch9.Models;

using System.Threading.Tasks;

namespace Ch9.ViewModels
{
    public class MovieGenreSettings2PageViewModel : ViewModelBase
    {
        private readonly IMovieGenreSettingsService _movieGenreSettingsService;

        public MovieGenreSettingsModel MovieGenreSettings { get; private set; }

        public MovieGenreSettings2PageViewModel(MovieGenreSettingsModel movieGenreSettings, IMovieGenreSettingsService movieGenreSettingsService, IPageService pageService) : base(pageService)
        {
            MovieGenreSettings = movieGenreSettings;
            _movieGenreSettingsService = movieGenreSettingsService;
        }

        public async Task PersistMovieGenreSettings() => await _movieGenreSettingsService.SaveGenreSelection(MovieGenreSettings);        

    }
}

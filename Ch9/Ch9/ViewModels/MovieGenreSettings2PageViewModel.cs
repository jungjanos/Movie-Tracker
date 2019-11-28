using Ch9.Models;
using Ch9.Services;

namespace Ch9.ViewModels
{
    public class MovieGenreSettings2PageViewModel : ViewModelBase
    {
        public MovieGenreSettings MovieGenreSettings { get; private set; }

        public MovieGenreSettings2PageViewModel(MovieGenreSettings movieGenreSettings, IPageService pageService) : base(pageService)
        {
            MovieGenreSettings = movieGenreSettings;
        }

    }
}

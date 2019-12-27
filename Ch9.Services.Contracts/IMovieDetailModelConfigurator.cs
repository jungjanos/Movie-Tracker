using Ch9.Ui.Contracts.Models;

using System.Collections.Generic;

namespace Ch9.Services.Contracts
{
    public interface IMovieDetailModelConfigurator
    {
        void SetGalleryImageSources(MovieDetailModel movie);
        void SetGenreNamesFromGenreIds(IEnumerable<MovieDetailModel> movies);
        void SetImageSrc(IEnumerable<MovieDetailModel> movies);
    }
}

using Ch9.ApiClient;
using Ch9.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ch9.Utils
{
    public interface IMovieDetailModelConfigurator
    {
        void SetGalleryImageSources(MovieDetailModel movie);
        void SetGenreNamesFromGenreIds(IEnumerable<MovieDetailModel> movies);
        void SetImageSrc(IEnumerable<MovieDetailModel> movies);
    }

    // TODO : This class should be composed into MovieDetailModel (static field ?)
    public class MovieDetailModelConfigurator : IMovieDetailModelConfigurator
    {
        private readonly TmdbConfigurationModel tmdbConfiguration;
        private readonly MovieGenreSettings movieGenreSettings;

        public MovieDetailModelConfigurator(TmdbConfigurationModel tmdbConfiguration, MovieGenreSettings movieGenreSettings)
        {
            this.tmdbConfiguration = tmdbConfiguration;
            this.movieGenreSettings = movieGenreSettings;
        }

        public void SetGalleryImageSources(MovieDetailModel movie)
        {
            List<string> tempResult = new List<string>(1 + movie.ImageDetailCollection.Backdrops?.Length ?? 0);

            tempResult.Add(movie.ImgBackdropSrc);

            if (movie.ImageDetailCollection.Posters?.Length > 0)
                tempResult.Add(tmdbConfiguration.Images.BaseUrl + "w780" + movie.ImageDetailCollection.Posters.First().FilePath);

            if (movie.ImageDetailCollection.Backdrops?.Length > 0)
                foreach (ImageModel backdrop in movie.ImageDetailCollection.Backdrops)
                    tempResult.Add(tmdbConfiguration.Images.BaseUrl + "w780" + backdrop.FilePath);

            movie.GalleryDisplayImages = tempResult.ToArray();
        }


        public void SetImageSrc(IEnumerable<MovieDetailModel> movies)
        {
            foreach (MovieDetailModel movie in movies)
            {
                movie.ImgSmSrc = tmdbConfiguration.Images.BaseUrl + tmdbConfiguration.Images.PosterSizes[0] + movie.ImgPosterName;
                movie.ImgBackdropSrc = tmdbConfiguration.Images.BaseUrl + "w780" + movie.ImgBackdropName;
                movie.GalleryDisplayImages = new string[]
                {
                    tmdbConfiguration.Images.BaseUrl + "w780" + movie.ImgBackdropName
                };
                movie.GalleryDisplayImage = movie.GalleryDisplayImages[0];
            }
        }

        public void SetGenreNamesFromGenreIds(IEnumerable<MovieDetailModel> movies)
        {
            foreach (MovieDetailModel movie in movies)
            {
                movie.Genre = movie.GenreIds?.Length == null ? string.Empty :
                string.Join(", ", movie.GenreIds.Select(id => movieGenreSettings.GenreSelectionDisplay.FirstOrDefault(y => y.Id == id)?.GenreName ?? string.Empty))
                .TrimEnd(new char[] { ',', ' ' });
            }
        }
    }
}

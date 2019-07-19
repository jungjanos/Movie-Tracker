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
            if (movie.GalleryDisplayImages.Count < 2)
            {                
                if (movie.ImageDetailCollection.Backdrops?.Length > 0)
                {
                    foreach (ImageModel backdrop in movie.ImageDetailCollection.Backdrops.Skip(1))
                        movie.GalleryDisplayImages.Add(tmdbConfiguration.Images.BaseUrl + tmdbConfiguration.Images.BackdropSizes[1] + backdrop.FilePath);
                }
                else if (movie.ImageDetailCollection.Posters?.Length > 0)
                {
                    foreach (ImageModel poster in movie.ImageDetailCollection.Posters.Skip(1))
                        movie.GalleryDisplayImages.Add(tmdbConfiguration.Images.BaseUrl + tmdbConfiguration.Images.PosterSizes.Last() + poster.FilePath);
                }                
            }                
        }

        public void SetImageSrc(IEnumerable<MovieDetailModel> movies)
        {
            foreach (MovieDetailModel movie in movies)
            {                
                if (movie.GalleryDisplayImages.Count == 0)
                {
                    movie.ImgSmSrc = tmdbConfiguration.Images.BaseUrl + tmdbConfiguration.Images.PosterSizes[0] + movie.ImgPosterName;

                    if (!string.IsNullOrEmpty(movie.ImgBackdropName))
                        movie.GalleryDisplayImages.Add(tmdbConfiguration.Images.BaseUrl + tmdbConfiguration.Images.BackdropSizes[1] + movie.ImgBackdropName);

                    else if (!string.IsNullOrEmpty(movie.ImgPosterName))
                        movie.GalleryDisplayImages.Add(tmdbConfiguration.Images.BaseUrl + tmdbConfiguration.Images.PosterSizes.Last() + movie.ImgPosterName);
                }
            }
        }

        public void SetGenreNamesFromGenreIds(IEnumerable<MovieDetailModel> movies)
        {
            foreach (MovieDetailModel movie in movies)
            {
                if (movie.Adult)
                    movie.Genre = "Porn";
                else if (movie.GenreIds == null)
                    movie.Genre = string.Empty;
                else
                    movie.Genre = string.Join(", ", movie.GenreIds.Select(id => movieGenreSettings.GenreSelectionDisplay.FirstOrDefault(y => y.Id == id)?.GenreName ?? string.Empty)).TrimEnd(new char[] { ',', ' ' });
            }
        }
    }
}

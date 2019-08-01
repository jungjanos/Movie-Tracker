using Ch9.Models;
using System.Collections.Generic;
using System.Linq;

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
        private readonly ISettings _settings;
        private readonly TmdbConfigurationModel _tmdbConfiguration;
        private readonly MovieGenreSettings _movieGenreSettings;

        private string ImageBaseUrl => _settings.UseHttpsForImages? _tmdbConfiguration.Images.SecureBaseUrl : _tmdbConfiguration.Images.BaseUrl;

        public MovieDetailModelConfigurator(ISettings settings, TmdbConfigurationModel tmdbConfiguration, MovieGenreSettings movieGenreSettings)
        {
            _settings = settings;
            _tmdbConfiguration = tmdbConfiguration;
            _movieGenreSettings = movieGenreSettings;
        }

        public void SetGalleryImageSources(MovieDetailModel movie)
        {
            string baseUrl = ImageBaseUrl;

            if (movie.MovieImages.Count < 2)
            {
                if (movie.ImageDetailCollection.Backdrops?.Length > 0)
                {
                    foreach (ImageModel backdrop in movie.ImageDetailCollection.Backdrops.Skip(1))
                    {
                        backdrop.FilePath = baseUrl + _tmdbConfiguration.Images.BackdropSizes[1] + backdrop.FilePath;
                        movie.MovieImages.Add(backdrop);
                    }
                }
                else if (movie.ImageDetailCollection.Posters?.Length > 0)
                {
                    foreach (ImageModel poster in movie.ImageDetailCollection.Posters.Skip(1))
                    {
                        poster.FilePath = baseUrl + _tmdbConfiguration.Images.PosterSizes.Last() + poster.FilePath;
                        movie.MovieImages.Add(poster);
                    }
                }
            }
        }

        public void SetImageSrc(IEnumerable<MovieDetailModel> movies)
        {
            string baseUrl = ImageBaseUrl;

            foreach (MovieDetailModel movie in movies)
            {
                movie.ImgSmSrc = baseUrl + _tmdbConfiguration.Images.PosterSizes[0] + movie.ImgPosterName;
                if (movie.MovieImages.Count == 0)
                {
                    var firstDisplayImage = new ImageModel();

                    if (!string.IsNullOrEmpty(movie.ImgBackdropName))
                    {
                        firstDisplayImage.FilePath = baseUrl + _tmdbConfiguration.Images.BackdropSizes[1] + movie.ImgBackdropName;
                        movie.MovieImages.Add(firstDisplayImage);
                    }                       
                    else if (!string.IsNullOrEmpty(movie.ImgPosterName))
                    {
                        firstDisplayImage.FilePath = baseUrl + _tmdbConfiguration.Images.PosterSizes.Last() + movie.ImgPosterName;
                        movie.MovieImages.Add(firstDisplayImage);
                    }
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
                    movie.Genre = string.Join(", ", movie.GenreIds.Select(id => _movieGenreSettings.GenreSelectionDisplay.FirstOrDefault(y => y.Id == id)?.GenreName ?? string.Empty)).TrimEnd(new char[] { ',', ' ' });
            }
        }
    }
}

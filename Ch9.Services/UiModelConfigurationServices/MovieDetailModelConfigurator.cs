﻿using Ch9.Models;
using Ch9.Services.Contracts;

using System.Collections.Generic;
using System.Linq;

namespace Ch9.Services.UiModelConfigurationServices
{    
    public class MovieDetailModelConfigurator : IMovieDetailModelConfigurator
    {
        private readonly ISettings _settings;
        private TmdbConfigurationModel _tmdbConfiguration => _tmdbConfigurationCache.TmdbConfigurationModel;
        private ITmdbConfigurationCache _tmdbConfigurationCache;
        private readonly MovieGenreSettingsModel _movieGenreSettings;

        private string ImageBaseUrl => _settings.UseHttpsForImages ? _tmdbConfiguration.Images.SecureBaseUrl : _tmdbConfiguration.Images.BaseUrl;

        public MovieDetailModelConfigurator(ISettings settings, ITmdbConfigurationCache tmdbConfigurationCache, MovieGenreSettingsModel movieGenreSettings)
        {
            _settings = settings;
            _tmdbConfigurationCache = tmdbConfigurationCache;
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
                        firstDisplayImage.FilePath = baseUrl + _tmdbConfiguration.Images.BackdropSizes[1] + movie.ImgBackdropName;
                    else if (!string.IsNullOrEmpty(movie.ImgPosterName))
                        firstDisplayImage.FilePath = baseUrl + _tmdbConfiguration.Images.PosterSizes.Last() + movie.ImgPosterName;
                    else
                        firstDisplayImage.FilePath = @"https://invalidurl.jpg";

                    movie.MovieImages.Add(firstDisplayImage);
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
                    movie.Genre = string.Join(", ", movie.GenreIds.Select(id => _movieGenreSettings.UserGenreSelection.FirstOrDefault(y => y.Id == id)?.GenreName ?? string.Empty)).TrimEnd(new char[] { ',', ' ' });
            }
        }
    }
}

using Ch9.ApiClient;
using Ch9.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ch9.Utils
{
    public class ImagePathConfiguratorUtil
    {
        private readonly TmdbConfigurationModel tmdbConfiguration;

        public ImagePathConfiguratorUtil(TmdbConfigurationModel tmdbConfiguration)
        {
            this.tmdbConfiguration = tmdbConfiguration;
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

    }
}

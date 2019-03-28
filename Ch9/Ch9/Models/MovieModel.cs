using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Ch9.Models
{
    public class MovieModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("release_date")]
        public DateTime? ReleaseDate { get; set; }

        [JsonProperty("genre_ids")]
        public int[] GenreIds { get; set; }

        [JsonProperty("overview")]
        public string Overview { get; set; }


        [JsonProperty("poster_path")]
        public string ImgPosterName { get; set; }

        [JsonProperty("backdrop_path")]
        public string ImgBackdropName { get; set; }

        [JsonProperty("popularity")]
        public double Popularity { get; set; }

        [JsonProperty("vote_count")]
        public int VoteCount { get; set; }

        [JsonProperty("vote_average")]
        public double VoteAverage { get; set; }


        public string ImgSmSrc { get; set; }

        protected string imgBackdropSrc;
        public string ImgBackdropSrc { get; set; }    

        public int Year => ReleaseDate.HasValue ? ReleaseDate.Value.Year : -1;

        public string Genre
        { get; set; }
    }

    public class MovieDetailModel : MovieModel, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false;
            }
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }


        private int budget;
        [JsonProperty("budget")]
        public int Budget
        {
            get => budget;
            set => SetProperty(ref budget, value);

        }

        private string homepage;
        [JsonProperty("homepage")]
        public string Homepage
        {
            get => homepage;
            set => SetProperty(ref homepage, value);
        }

        [JsonProperty("imdb_id")]
        public string ImdbId { get; set; }


        private ProductionCountry[] productionCountries;
        [JsonProperty("production_countries")]
        public ProductionCountry[] ProductionCountries
        {
            get => productionCountries;
            set
            {
                SetProperty(ref productionCountries, value);
                Countries = string.Join(", ", productionCountries.Select(x => x.Iso/* == "US" ? "USA" : x.Name*/));
                OnPropertyChanged(nameof(Countries));
            }
        }

        public string Countries { get; set; }


        private int? duration;
        [JsonProperty("runtime")]
        public int? Duration
        {
            get => duration;
            set => SetProperty(ref duration, value);
        }


        private string tagline;
        [JsonProperty("tagline")]
        public string Tagline
        {
            get => tagline;
            set => SetProperty(ref tagline, value);
        }

        private ImageDetailCollection imageDetailCollection;
        public ImageDetailCollection ImageDetailCollection
        {
            get => imageDetailCollection;
            set => SetProperty(ref imageDetailCollection, value);
        }

        private string[] galleryDisplayImages;
        public string[] GalleryDisplayImages
        {
            get => galleryDisplayImages;
            set
            {
                SetProperty(ref galleryDisplayImages, value);
            }
        }

        // this is a counter which always displays one more than it stores!
        // and which always stores one less than it receives!
        // reason: to make model binding to UI simpler
        private int galleryPositionCounter;        
        public int GalleryPositionCounter
        {
            get => galleryPositionCounter+1;
            set
            {
                int circularIndex = value % GalleryDisplayImages.Length-1;
                if (circularIndex < 0)
                    circularIndex += GalleryDisplayImages.Length;

                if (SetProperty(ref galleryPositionCounter, circularIndex))
                {
                    GalleryDisplayImage = GalleryDisplayImages[circularIndex];
                }
            }
        }

        private string galleryDisplayImage;
        public string GalleryDisplayImage
        {
            get => galleryDisplayImage;
            set => SetProperty(ref galleryDisplayImage, value);
        }
    }

    public class ProductionCountry
    {
        [JsonProperty("iso_3166_1")]
        public string Iso { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class ImageDetailCollection
    {
        [JsonProperty("backdrops")]
        public ImageModel[] Backdrops { get; set; }

        [JsonProperty("posters")]
        public ImageModel[] Posters { get; set; }
    }


    public class MovieModelComparer : IEqualityComparer<MovieModel>
    {
        public bool Equals(MovieModel x, MovieModel y)
        {
            if (x == null || y == null)
                return false;
            else
                return x.Id == y.Id;
        }

        public int GetHashCode(MovieModel obj)
        {
            return obj.Id;
        }
    }

    [Flags]
    public enum MovieCategories
    {
        
    }
}



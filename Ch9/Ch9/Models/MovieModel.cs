using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Ch9.Models
{
    // TODO : replace references to MovieModel to MovieDetailModel in code
    public class MovieModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("adult")]
        public bool Adult { get; set; }

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
        
        public string ImgBackdropSrc { get; set; }    

        public int Year => ReleaseDate.HasValue ? ReleaseDate.Value.Year : -1;

        public string Genre
        { get; set; }
    }

    public class MovieDetailModel : MovieModel, INotifyPropertyChanged
    {
        public MovieDetailModel()
        {
            _imageDetailCollection = new ImageDetailCollection();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private int _budget;
        [JsonProperty("budget")]
        public int Budget
        {
            get => _budget;
            set => SetProperty(ref _budget, value);

        }

        private string _homepage;
        [JsonProperty("homepage")]
        public string Homepage
        {
            get => _homepage;
            set => SetProperty(ref _homepage, value);
        }

        [JsonProperty("imdb_id")]
        public string ImdbId { get; set; }


        private ProductionCountry[] _productionCountries;
        [JsonProperty("production_countries")]
        public ProductionCountry[] ProductionCountries
        {
            get => _productionCountries;
            set
            {
                SetProperty(ref _productionCountries, value);
                Countries = string.Join(", ", _productionCountries.Select(x => x.Iso/* == "US" ? "USA" : x.Name*/));
                OnPropertyChanged(nameof(Countries));
            }
        }

        public string Countries { get; set; }


        private int? _duration;
        [JsonProperty("runtime")]
        public int? Duration
        {
            get => _duration;
            set => SetProperty(ref _duration, value);
        }


        private string _tagline;
        [JsonProperty("tagline")]
        public string Tagline
        {
            get => _tagline;
            set => SetProperty(ref _tagline, value);
        }

        private ImageDetailCollection _imageDetailCollection;
        public ImageDetailCollection ImageDetailCollection
        {
            get => _imageDetailCollection;
            set => SetProperty(ref _imageDetailCollection, value);
        }

        private string[] _galleryDisplayImages;
        public string[] GalleryDisplayImages
        {
            get => _galleryDisplayImages;
            set => SetProperty(ref _galleryDisplayImages, value);
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

        private string _galleryDisplayImage;
        public string GalleryDisplayImage
        {
            get => _galleryDisplayImage;
            set => SetProperty(ref _galleryDisplayImage, value);
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
}



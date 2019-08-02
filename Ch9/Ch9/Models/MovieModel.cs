using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Collections.ObjectModel;

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

        public int? Year => ReleaseDate.HasValue ? (ReleaseDate.Value.Year) as int? : null;

        public string Genre { get; set; }
    }

    public class MovieDetailModel : MovieModel, INotifyPropertyChanged
    {
        public MovieDetailModel()
        {
            ImageDetailCollection = new ImageDetailCollection();
            _movieImages = new ObservableCollection<ImageModel>();
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
                Countries = string.Join(", ", _productionCountries.Select(x => x.Iso));
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

        public ImageDetailCollection ImageDetailCollection { get; set; }

        private ObservableCollection<ImageModel> _movieImages;
        public ObservableCollection<ImageModel> MovieImages
        {
            get => _movieImages;
            set => SetProperty(ref _movieImages, value);
        }

        private ObservableCollection<ImageModel> _videoThumbnails;
        public ObservableCollection<ImageModel> VideoThumbnails
        {
            get => _videoThumbnails;
            set => SetProperty(ref _videoThumbnails, value);
        }

        private List<Review> _reviews;
        public List<Review> Reviews
        {
            get => _reviews;
            set => SetProperty(ref _reviews, value);
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
    }

    public class ProductionCountry
    {
        [JsonProperty("iso_3166_1")]
        public string Iso { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
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

        public int GetHashCode(MovieModel obj) => obj.Id;
    }

    public class MovieYearDescComparer : Comparer<MovieModel>
    {
        public override int Compare(MovieModel x, MovieModel y)
        {
            if (x.Year.HasValue && y.Year.HasValue)
                return x.Year.Value.CompareTo(y.Year.Value) * -1;
            else if (x.Year.HasValue)
                return -1;
            else return 1;
        }
    }
}



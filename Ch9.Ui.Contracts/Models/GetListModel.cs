using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Ch9.Ui.Contracts.Models
{    
    public class GetListsModel
    {
        [JsonProperty("page")]
        public int Page { get; set; }

        [JsonProperty("results")]
        public MovieListModel[] MovieLists { get; set; }

        [JsonProperty("total_pages")]
        public int TotalPages { get; set; }

        [JsonProperty("total_results")]
        public int TotalResults { get; set; }
    }

    // TODO: check whether INotifyPropertyCahnged interface can be removed
    public class MovieListModel : INotifyPropertyChanged
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("favorite_count")]
        public int FavoriteCount { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        private int _itemCount;
        [JsonProperty("item_count")]
        public int ItemCount
        {
            get => _itemCount;
            set => SetProperty(ref _itemCount, value);
        }

        private ObservableCollection<MovieDetailModel> _movies;       
        [JsonProperty("items")]
        public ObservableCollection<MovieDetailModel> Movies
        {
            get => _movies;
            set => SetProperty(ref _movies, value);
        }

        [JsonProperty("iso_639_1")]
        public string Iso639 { get; set; }

        [JsonProperty("list_type")]
        public string ListType { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("poster_path")]
        public string PosterPath { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }

    public class ListCrudResponseModel
    {
        [JsonProperty("status_message")]
        public string StatusMessage { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("status_code")]
        public int StatusCode { get; set; }

        [JsonProperty("list_id")]
        public int ListId { get; set; }
    }

    public class MovieListModelComparer : IEqualityComparer<MovieListModel>
    {
        public bool Equals(MovieListModel x, MovieListModel y)
        {
            if (x == null || y == null)
                return false;
            else
                return x.Id == y.Id;
        }

        public int GetHashCode(MovieListModel obj) => obj.Id;
    }

}

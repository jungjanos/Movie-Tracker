using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Ch9.Models
{
    public class SearchResult : INotifyPropertyChanged
    {
        [JsonProperty("page")]
        public int Page { get; set; }

        private ObservableCollection<MovieDetailModel> _movieDetailModels;
        [JsonProperty("results")]
        public ObservableCollection<MovieDetailModel> MovieDetailModels
        {
            get => _movieDetailModels;
            set => SetProperty(ref _movieDetailModels, value);
        }

        private int _totalResults;
        [JsonProperty("total_results")]
        public int TotalResults
        {
            get => _totalResults;
            set => SetProperty(ref _totalResults, value);
        }

        [JsonProperty("total_pages")]
        public int TotalPages { get; set; }

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
}

using Ch9.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;

namespace Ch9.ApiClient
{
    public class TmdbResponseBase
    {
        public HttpStatusCode HttpStatusCode { get; set; }
        public string Json { get; set; }
    }

    public class TmdbConfigurationModelResult : TmdbResponseBase
    { }

    public class GenreNameFetchResult : TmdbResponseBase
    { }

    public class SearchResult : TmdbResponseBase, INotifyPropertyChanged
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

        [JsonProperty("total_results")]
        public int TotalResults { get; set; }

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

    public class SearchByMovieResult : TmdbResponseBase
    { }

    // The two classes use the same result format,
    // inheritacne is used only to give a more descriptive class name
    public class TrendingMoviesResult : SearchByMovieResult { }

    public class FetchMovieDetailsResult : TmdbResponseBase
    { }

    public class GetMovieImagesResult : TmdbResponseBase
    { }
    public class GetMovieVideosResult :TmdbResponseBase
    { }

    public class GetMovieRecommendationsResult : TmdbResponseBase
    { }

    public class GetSimilarMoviesResult : TmdbResponseBase
    { }

    public class CreateRequestTokenResult : TmdbResponseBase
    { }

    public class CreateSessionIdResult : TmdbResponseBase
    { }

    public class DeleteSessionResult : TmdbResponseBase
    { }

    public class GetAccountDetailsResult : TmdbResponseBase
    { }

    public class GetListsResult : TmdbResponseBase
    { }

    public class CreateListResult : TmdbResponseBase
    { }

    public class DeleteListResult : TmdbResponseBase
    { }

    public class GetListDetailsResult : TmdbResponseBase
    { }

    public class AddMovieResult : TmdbResponseBase
    { }

    public class RemoveMovieResult : TmdbResponseBase
    { }

    public class GetMovieReviewsResult : TmdbResponseBase
    { }

    public class GetMovieWatchlistResult : TmdbResponseBase
    { }

    public class GetFavoriteMoviesResult : TmdbResponseBase
    { }

    public class UpdateWatchlistResult : TmdbResponseBase
    { }

    public class UpdateFavoriteListResult : TmdbResponseBase
    { }

    public class RateMovieResult : TmdbResponseBase
    { }

    public class DeleteMovieRatingResult : TmdbResponseBase
    { }

    public class GetMovieCreditsResult : TmdbResponseBase
    { }

    // Because of the flexible object type of the Json object the WebAPI sends back as response,
    // The Api client itself handles the deserialization of the server's response.
    // this behavior diverges from normal case
    public class GetAccountMovieStatesResult : TmdbResponseBase
    {
        public AccountMovieStates States { get; set; }
    }

    public enum Rating
    {
        Half = 1,
        One = 2,
        OneAndHalf = 3,
        Two = 4,
        TwoAndHalf = 5,
        Three = 6,
        ThreeAndHalf = 7,
        Four = 8,
        FourAndHalf = 9,
        Five = 10,
        FiveAndHalf = 11,
        Six = 12,
        SixAndHalf = 13,
        Seven = 14,
        SevenAndHalf = 15,
        Eight = 16,
        EightAndHalf = 17,
        Nine = 18,
        NineAndHalf = 19,
        Ten = 20
    }

    public class AccountMovieStates
    {
        [JsonProperty("id")]
        public int MovieId { get; set; }

        [JsonProperty("favorite")]
        public bool IsFavorite { get; set; }

        [JsonProperty("rated")]
        public RatingWrapper Rating { get; set; }
        [JsonProperty("watchlist")]
        public bool OnWatchlist { get; set; }
    }

    public class RatingWrapper
    {
        [JsonProperty("value")]
        public decimal Value { get; set; }
    }

}

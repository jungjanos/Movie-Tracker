using Ch9.Ui.Contracts.Models;
using System.Net;

namespace Ch9.Services.Contracts
{
    public abstract class ApiCommunicationServiceResponseBase
    {
        public HttpStatusCode HttpStatusCode { get; protected  set; }
    }

    public class TryGetMovieImagesResponse : ApiCommunicationServiceResponseBase
    {        
        public ImageDetailCollection ImageDetailCollection { get; private set; }

        public TryGetMovieImagesResponse(HttpStatusCode statusCode, ImageDetailCollection imageDetailCollection)
        {
            HttpStatusCode = statusCode;
            ImageDetailCollection = imageDetailCollection;
        }
    }

    public class TryGetMovieCreditsResponse : ApiCommunicationServiceResponseBase
    {
        public MovieCreditsModel MovieCreditsModel { get; private set; }

        public TryGetMovieCreditsResponse(HttpStatusCode statusCode, MovieCreditsModel movieCredits)
        {
            HttpStatusCode = statusCode;
            MovieCreditsModel = movieCredits;
        }
    }

    public class TryGetPersonsDetailsResponse : ApiCommunicationServiceResponseBase
    {
        public PersonsDetailsModel PersonsDetailsModel { get; private set; }

        public TryGetPersonsDetailsResponse(HttpStatusCode statusCode, PersonsDetailsModel personsDetails)
        {
            HttpStatusCode = statusCode;
            PersonsDetailsModel = personsDetails;
        }
    }

    public class TrySearchByMovieResponse : ApiCommunicationServiceResponseBase
    {
        public SearchResult SearchResult{ get; private set; }

        public TrySearchByMovieResponse (HttpStatusCode statusCode, SearchResult searchResult)
        {
            HttpStatusCode = statusCode;
            SearchResult = searchResult;
        }
    }

    public class TryGetPersonImagesResponse : ApiCommunicationServiceResponseBase
    {
        public ImageModel[] Images{ get; private set; }

        public TryGetPersonImagesResponse(HttpStatusCode statusCode, ImageModel[] images)
        {
            HttpStatusCode = statusCode;
            Images = images;
        }
    }

    public class TryGetPersonsMovieCreditsResponse : ApiCommunicationServiceResponseBase
    {
        public PersonsMovieCreditsModel PersonsMovieCreditsModel{ get; private set; }

        public TryGetPersonsMovieCreditsResponse(HttpStatusCode statusCode, PersonsMovieCreditsModel personsMovieCreditsModel)
        {
            HttpStatusCode = statusCode;
            PersonsMovieCreditsModel = personsMovieCreditsModel;
        }
    }

    public class TryGetTrendingMoviesResponse : ApiCommunicationServiceResponseBase
    {
        public SearchResult TrendingMovies { get; private set; }

        public TryGetTrendingMoviesResponse(HttpStatusCode statusCode, SearchResult trendingMovies)
        {
            HttpStatusCode = statusCode;
            TrendingMovies = trendingMovies;
        }
    }

    public class TryGetMovieReviewsResponse : ApiCommunicationServiceResponseBase
    {
        public ReviewsModel ReviewsModel { get; private set; }

        public TryGetMovieReviewsResponse(HttpStatusCode statusCode, ReviewsModel reviewsModel)
        {
            HttpStatusCode = statusCode;
            ReviewsModel = reviewsModel;
        }
    }

    public class TryDeleteMovieRatingResponse : ApiCommunicationServiceResponseBase
    {
        public TryDeleteMovieRatingResponse(HttpStatusCode statusCode) => HttpStatusCode = statusCode;
    }

    public class TryRateMovieResponse : ApiCommunicationServiceResponseBase
    {
        public TryRateMovieResponse(HttpStatusCode statusCode) => HttpStatusCode = statusCode;
    }

    public class TryGetMovieRecommendationsResponse : ApiCommunicationServiceResponseBase
    {
        public SearchResult MovieRecommendations { get; private set; }

        public TryGetMovieRecommendationsResponse(HttpStatusCode statusCode, SearchResult movieRecommendations)
        {
            HttpStatusCode = statusCode;
            MovieRecommendations = movieRecommendations;
        }
    }

    public class TryGetSimilarMoviesResponse : ApiCommunicationServiceResponseBase
    {
        public SearchResult SimilarMovies { get; private set; }

        public TryGetSimilarMoviesResponse(HttpStatusCode statusCode, SearchResult similarMovies)
        {
            HttpStatusCode = statusCode;
            SimilarMovies = similarMovies;
        }
    }

    public class TryGetMovieDetailsWithAccountStatesResponse : ApiCommunicationServiceResponseBase
    {
        public AccountMovieStates AccountMovieStates { get; private set; }
        public TryGetMovieDetailsWithAccountStatesResponse(HttpStatusCode statusCode, AccountMovieStates accountMovieStates)
        {
            HttpStatusCode = statusCode;
            AccountMovieStates = accountMovieStates;
        }
    }

    public class TryDeleteSessionResponse : ApiCommunicationServiceResponseBase
    {
        public TryDeleteSessionResponse(HttpStatusCode statusCode)
        {
            HttpStatusCode = statusCode;
        }
    }
}

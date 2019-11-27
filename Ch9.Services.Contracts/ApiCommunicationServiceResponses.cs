using Ch9.Ui.Contracts.Models;
using System.Net;

namespace Ch9.Services.Contracts
{
    public class ApiCommunicationServiceResponseBase
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
        public SearchResult SearchResult { get; private set; }

        public TryGetTrendingMoviesResponse(HttpStatusCode statusCode, SearchResult searchResult)
        {
            HttpStatusCode = statusCode;
            SearchResult = searchResult;
        }
    }
}

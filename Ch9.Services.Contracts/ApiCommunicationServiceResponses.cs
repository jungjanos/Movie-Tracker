﻿using Ch9.Models;

using System.Net;

namespace Ch9.Services.Contracts
{
    public abstract class ApiCommunicationServiceResponseBase
    {
        public HttpStatusCode HttpStatusCode { get; protected set; }
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
        public SearchResult SearchResult { get; private set; }

        public TrySearchByMovieResponse(HttpStatusCode statusCode, SearchResult searchResult)
        {
            HttpStatusCode = statusCode;
            SearchResult = searchResult;
        }
    }

    public class TryGetPersonImagesResponse : ApiCommunicationServiceResponseBase
    {
        public ImageModel[] Images { get; private set; }

        public TryGetPersonImagesResponse(HttpStatusCode statusCode, ImageModel[] images)
        {
            HttpStatusCode = statusCode;
            Images = images;
        }
    }

    public class TryGetPersonsMovieCreditsResponse : ApiCommunicationServiceResponseBase
    {
        public PersonsMovieCreditsModel PersonsMovieCreditsModel { get; private set; }

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

    public class TryCreateRequestTokenResponse : ApiCommunicationServiceResponseBase
    {
        public RequestToken RequestToken { get; private set; }

        public TryCreateRequestTokenResponse(HttpStatusCode statusCode, RequestToken requestToken)
        {
            HttpStatusCode = statusCode;
            RequestToken = requestToken;
        }
    }

    public class TryValidateRequestTokenWithLoginResponse : ApiCommunicationServiceResponseBase
    {
        public RequestToken RequestToken { get; private set; }
        public TryValidateRequestTokenWithLoginResponse(HttpStatusCode statusCode, RequestToken requestToken)
        {
            HttpStatusCode = statusCode;
            RequestToken = requestToken;
        }
    }

    public class TryCreateSessionIdResponse : ApiCommunicationServiceResponseBase
    {
        public SessionIdResponseModel SessionIdResponseModel { get; private set; }

        public TryCreateSessionIdResponse(HttpStatusCode statusCode, SessionIdResponseModel sessionIdResponseModel)
        {
            HttpStatusCode = statusCode;
            SessionIdResponseModel = sessionIdResponseModel;
        }
    }


    public class TryGetTmdbConfigurationResponse : ApiCommunicationServiceResponseBase
    {
        public TmdbConfigurationModel ConfigurationModel { get; private set; }

        public TryGetTmdbConfigurationResponse(HttpStatusCode statusCode, TmdbConfigurationModel configurationModel)
        {
            HttpStatusCode = statusCode;
            ConfigurationModel = configurationModel;
        }
    }

    public class TryGetGenreIdsWithNamesResponse : ApiCommunicationServiceResponseBase
    {
        public GenreIdNamePairs GenreIdNamePairs { get; private set; }

        public TryGetGenreIdsWithNamesResponse(HttpStatusCode statusCode, GenreIdNamePairs genreIdNamePairs)
        {
            HttpStatusCode = statusCode;
            GenreIdNamePairs = genreIdNamePairs;
        }
    }

    public class TryGetFavoriteMoviesResponse : ApiCommunicationServiceResponseBase
    {
        public SearchResult FavoriteMovies { get; private set; }

        public TryGetFavoriteMoviesResponse(HttpStatusCode statusCode, SearchResult favoriteMovies)
        {
            HttpStatusCode = statusCode;
            FavoriteMovies = favoriteMovies;
        }
    }

    public class TryUpdateFavoriteListResponse : ApiCommunicationServiceResponseBase
    {
        public TryUpdateFavoriteListResponse(HttpStatusCode statusCode)
        {
            HttpStatusCode = statusCode;
        }
    }

    public class TryGetMovieWatchlistResponse : ApiCommunicationServiceResponseBase
    {
        public SearchResult MoviesOnWatchlist { get; private set; }

        public TryGetMovieWatchlistResponse(HttpStatusCode statusCode, SearchResult moviesOnWatchlist)
        {
            HttpStatusCode = statusCode;
            MoviesOnWatchlist = moviesOnWatchlist;
        }
    }

    public class TryUpdateWatchlistResponse : ApiCommunicationServiceResponseBase
    {
        public TryUpdateWatchlistResponse(HttpStatusCode statusCode)
        {
            HttpStatusCode = statusCode;
        }
    }

    public class TryGetListsResponse : ApiCommunicationServiceResponseBase
    {
        public GetListsModel ListsModel { get; private set; }

        public TryGetListsResponse(HttpStatusCode statusCode, GetListsModel listsModel)
        {
            HttpStatusCode = statusCode;
            ListsModel = listsModel;
        }
    }

    public class TryGetListDetailsResponse : ApiCommunicationServiceResponseBase
    {
        public MovieListModel ListDetails { get; private set; }

        public TryGetListDetailsResponse(HttpStatusCode statusCode, MovieListModel listDetails)
        {
            HttpStatusCode = statusCode;
            ListDetails = listDetails;
        }
    }

    public class TryCreateListResponse : ApiCommunicationServiceResponseBase
    {
        public ListCrudResponseModel ListCrudResponse { get; private set; }
        public TryCreateListResponse(HttpStatusCode statusCode, ListCrudResponseModel listCrudResponse)
        {
            HttpStatusCode = statusCode;
            ListCrudResponse = listCrudResponse;
        }
    }

    public class TryDeleteListResponse : ApiCommunicationServiceResponseBase
    {
        public TryDeleteListResponse(HttpStatusCode statusCode)
        {
            HttpStatusCode = statusCode;
        }
    }

    public class TryAddMovieResponse : ApiCommunicationServiceResponseBase
    {
        public TryAddMovieResponse(HttpStatusCode statusCode)
        {
            HttpStatusCode = statusCode;
        }
    }

    public class TryRemoveMovieResponse : ApiCommunicationServiceResponseBase
    {
        public TryRemoveMovieResponse(HttpStatusCode statusCode)
        {
            HttpStatusCode = statusCode;
        }
    }

    public class TryGetMovieVideosResponse : ApiCommunicationServiceResponseBase
    {
        public GetMovieVideosModel MovieVideosModel { get; private set; }

        public TryGetMovieVideosResponse(HttpStatusCode statusCode, GetMovieVideosModel movieVideosModel)
        {
            HttpStatusCode = statusCode;
            MovieVideosModel = movieVideosModel;
        }
    }
}

using Ch9.Services.Contracts;
using Ch9.Models;

using System.Collections.ObjectModel;

namespace Ch9.Services.MovieListServices
{
    public static class SearchResultExtensions
    {
        /// <summary>
        /// Not allowed to throw if targetList != NULL  
        /// Appends the movie collection in the "MovieDetailModels" property of the server response
        /// to the target's same named property. Updates the targets page and result counters from the serverResponse 
        /// </summary>
        /// <param name="targetList">this is the public observered collection wich is updated with data from the server's response</param>
        /// <param name="serverResponse">contains the server's response, containing movies to append to the observed collection and page/result counter</param>
        public static void AppendResult(this SearchResult targetList, SearchResult serverResponse, IMovieDetailModelConfigurator movieDetailConfigurator)
        {
            if (serverResponse.MovieDetailModels.Count > 0)
            {
                movieDetailConfigurator.SetImageSrc(serverResponse.MovieDetailModels);
                movieDetailConfigurator.SetGenreNamesFromGenreIds(serverResponse.MovieDetailModels);

                foreach (MovieDetailModel movie in serverResponse.MovieDetailModels)
                    targetList.MovieDetailModels.Add(movie);

                targetList.Page = serverResponse.Page;
                targetList.TotalPages = serverResponse.TotalPages;
                targetList.TotalResults = serverResponse.TotalResults;
            }
        }

        public static void AppendResult(this SearchResult targetList, SearchResult serverResponse)
        {
            if (serverResponse.MovieDetailModels.Count > 0)
            {
                foreach (MovieDetailModel movie in serverResponse.MovieDetailModels)
                    targetList.MovieDetailModels.Add(movie);

                targetList.Page = serverResponse.Page;
                targetList.TotalPages = serverResponse.TotalPages;
                targetList.TotalResults = serverResponse.TotalResults;
            }
        }


        /// <summary>
        /// Not allowed to throw. 
        /// Depending on the state of the MovieDetailModels property of the argument, if it was null, it will be initialized to an empty
        /// ObservableCollection, or if it was already initialized, it will be cleared to an empty state. 
        /// The pagination properties will all be set to zero. 
        /// </summary>        
        public static void InitializeOrClearMovieCollection(this SearchResult collection)
        {
            if (collection.MovieDetailModels == null)
                collection.MovieDetailModels = new ObservableCollection<MovieDetailModel>();
            else
                collection.MovieDetailModels.Clear();

            collection.Page = 0;
            collection.TotalPages = 0;
            collection.TotalResults = 0;
        }

        public static bool CanLoad(this SearchResult collection) => collection.Page == 0 ? false : collection.Page >= collection.TotalPages ? false : true;
    }
}

using Ch9.ApiClient;
using Ch9.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;

namespace Ch9.Utils
{
    public static class Utils
    {
        public static void UpdateListviewCollection<T>(ObservableCollection<T> oldList, IEnumerable<T> newList, IEqualityComparer<T> valueComparer)
        {
            if (newList == null)
                return;
            List<T> toRemove = oldList.Where(x => !newList.Contains(x, valueComparer)).ToList();
            List<T> toAdd = newList.Where(y => !oldList.Contains(y, valueComparer)).ToList();

            foreach (T item in toRemove)
                oldList.Remove(item);

            foreach (T item in toAdd)
                oldList.Add(item);
        }

        // Empties and refills an ObservableCollection<T> object without dropping the reference to it.  
        public static void RefillList<T>(ObservableCollection<T> intoList,  IEnumerable<T> fromList)
        {
            intoList.Clear();
            foreach (T item in fromList)
                intoList.Add(item);
        }

        public static bool IsSuccessCode(this HttpStatusCode httpStatusCode)
        {
            return (200 <= (int)httpStatusCode && (int)httpStatusCode < 300);
        }

        public static bool Is500Code(this HttpStatusCode httpStatusCode)
        {
            return 500 == (int)httpStatusCode;
        }

        public static decimal GetValue(this Rating rating) => (decimal)rating / 2;

        /// <summary>
        /// Not allowed to throw. 
        /// Appends the movie collection in the "MovieDetailModels" property of the server response
        /// to the target's same named property. Updates the targets page and result counters from the serverResponse 
        /// </summary>
        /// <param name="targetList">this is the public observered collection wich is updated with data from the server's response</param>
        /// <param name="serverResponse">contains the server's response, containing movies to append to the observed collection and page/result counter</param>
        public static void AppendResult(SearchResult targetList, SearchResult serverResponse, IMovieDetailModelConfigurator movieDetailConfigurator)
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
    }
}

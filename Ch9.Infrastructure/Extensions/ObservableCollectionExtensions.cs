using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Ch9.Infrastructure.Extensions
{
    public static class ObservableCollectionExtensions
    {
        /// <summary>
        /// Updates the collection based on the provided list. 
        /// </summary>        
        public static void UpdateObservableCollection<T>(this ObservableCollection<T> collection, IEnumerable<T> newList, IEqualityComparer<T> valueComparer)
        {
            if (newList == null)
                return;
            List<T> toRemove = collection.Where(x => !newList.Contains(x, valueComparer)).ToList();
            List<T> toAdd = newList.Where(y => !collection.Contains(y, valueComparer)).ToList();

            foreach (T item in toRemove)
                collection.Remove(item);

            foreach (T item in toAdd)
                collection.Add(item);
        }
    }
}

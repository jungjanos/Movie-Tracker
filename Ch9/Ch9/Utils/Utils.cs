using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Ch9.Utils
{
    public static class Utils
    {
        public static void UpdateListviewCollection<T>(ObservableCollection<T> oldList, IEnumerable<T> newList, IEqualityComparer<T> valueComparer)
        {
            List<T> toRemove = oldList.Where(x => !newList.Contains(x, valueComparer)).ToList();
            List<T> toAdd = newList.Where(y => !oldList.Contains(y, valueComparer)).ToList();

            foreach (T item in toRemove)
                oldList.Remove(item);

            foreach (T item in toAdd)
                oldList.Add(item);
        }
    }
}

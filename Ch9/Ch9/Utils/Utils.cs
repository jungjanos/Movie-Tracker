﻿using Ch9.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;

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
    }
}

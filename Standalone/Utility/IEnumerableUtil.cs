using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public static class IEnumerableUtil<T>
    {
        /// <summary>
        /// This is an iterator that will have no values.
        /// </summary>
        public static IEnumerable<T> EmptyIterator
        {
            get
            {
                yield break;
            }
        }

        public static IEnumerable<T> Iter(params T[] items)
        {
            foreach (T item in items)
            {
                yield return item;
            }
        }
    }

    public static class IEnumerableUtil
    {
        public static IEnumerable<T> AddSingle<T>(this IEnumerable<T> source, T single)
        {
            foreach (T item in source)
            {
                yield return item;
            }
            yield return single;
        }
    }
}

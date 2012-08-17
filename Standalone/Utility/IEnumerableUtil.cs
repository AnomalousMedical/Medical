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
                if (false)
                {
                    //A bit hacky, but this will product an empty iterator 
                    //to avoid having to check for null in instances where 
                    //it makes sense to return an empty collection.
                    yield return default(T);
                }
            }
        }
    }
}

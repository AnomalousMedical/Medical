using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public static class EmptyIterator
    {
        public static IEnumerable<T> Empty<T>()
        {
            yield break;
        }
    }
}

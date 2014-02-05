using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public static class IDisposableUtil
    {
        /// <summary>
        /// This can be called on any IDisposable instance, it will only call dispose on the instance if the instance is
        /// not null.
        /// </summary>
        /// <param name="disposable">The disposable to dispose, can be null.</param>
        public static void DisposeIfNotNull(IDisposable disposable)
        {
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        public static void DisposeIfNotNull(IDisposable first, IDisposable second)
        {
            if (first != null)
            {
                first.Dispose();
            }
            if (second != null)
            {
                second.Dispose();
            }
        }
    }
}

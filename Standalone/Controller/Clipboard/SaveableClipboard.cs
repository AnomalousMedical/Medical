using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical
{
    /// <summary>
    /// This allows anything that is Savable to be copied and pasted.
    /// </summary>
    public class SaveableClipboard
    {
        private CopySaver copySaver = new CopySaver();
        private Saveable sourceObject = null;

        public SaveableClipboard()
        {

        }

        /// <summary>
        /// Copy the passed object using a CopySaver and use that copy as a source object.
        /// </summary>
        /// <param name="toCopy">The object to copy for the source object.</param>
        public void copyToSourceObject(Saveable toCopy)
        {
            this.sourceObject = copySaver.copy<Saveable>(toCopy);
        }

        /// <summary>
        /// Create a copy to the type specified by T. Note that this will return
        /// null if sourceObject is not a T. It will also not actually be copied
        /// unless it can be converted to a T.
        /// </summary>
        /// <typeparam name="T">The type to copy to.</typeparam>
        /// <returns>A copy of sourceObject as a T or null if it could not be copied (for whatever reason).</returns>
        public T createCopy<T>()
            where T : Saveable
        {
            T copy = default(T);
            if (sourceObject != null)
            {
                if (sourceObject is T)
                {
                    copy = (T)copySaver.copy<Saveable>(sourceObject);
                }
            }
            return copy;
        }
    }
}

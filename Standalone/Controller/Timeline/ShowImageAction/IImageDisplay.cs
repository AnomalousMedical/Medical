using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Medical
{
    /// <summary>
    /// This interface abstracts how the timelines display images.
    /// </summary>
    interface IImageDisplay : IDisposable
    {
        /// <summary>
        /// Set the image for this IImageDisplay.
        /// </summary>
        /// <param name="image"></param>
        void setImage(Stream image);

        /// <summary>
        /// Show the IImageDisplay.
        /// </summary>
        void show();
    }
}

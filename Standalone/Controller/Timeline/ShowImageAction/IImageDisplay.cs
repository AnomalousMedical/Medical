using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Engine;

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

        /// <summary>
        /// The location of the image. This is expressed as a floating point value between 0 and 1.
        /// </summary>
        Vector2 Position { get; set; }

        /// <summary>
        /// The size of the image. This is expressed as a floating point value between 0 and 1.
        /// </summary>
        Size2 Size { get; set; }

        /// <summary>
        /// Specify whether to keep the original aspect ratio of the image or not.
        /// </summary>
        bool KeepAspectRatio { get; set; }
    }
}

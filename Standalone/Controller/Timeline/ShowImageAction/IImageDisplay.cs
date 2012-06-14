using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Engine;
using Engine.Attributes;

namespace Medical
{
    [SingleEnum]
    public enum ImageAlignment
    {
        LeftTop = 0,
        LeftBottom = 1,
        RightTop = 2,
        RightBottom = 3,
        TopCenter = 4,
        BottomCenter = 5,
        LeftCenter = 6,
        RightCenter = 7,
        Center = 8,
        Specify = 9,
    }

    /// <summary>
    /// This interface abstracts how the timelines display images.
    /// </summary>
    public interface IImageDisplay : IDisposable
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

        /// <summary>
        /// Specify the alignment of the image
        /// </summary>
        ImageAlignment Alignment { get; set; }

        /// <summary>
        /// Suppress the layout. Use this when setting multiple properties at once.
        /// </summary>
        bool SuppressLayout { get; set; }

        /// <summary>
        /// If you used SuppressLayout and want to explicitly layout the control. SuppressLayout must be false or this will still do nothing.
        /// </summary>
        void layout();
    }
}

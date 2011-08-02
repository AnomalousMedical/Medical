using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Engine;

namespace Medical
{
    public enum TextualAlignment
    {
        HCenter = 0, /**< center horizontally */
        VCenter = 0, /**< center vertically */
        Center = HCenter | VCenter, /**< center in the dead center */

        Left = 1 << (1), /**< value from the left (and center vertically) */
        Right = 1 << (2), /**< value from the right (and center vertically) */
        HStretch = Left | Right, /**< stretch horizontally proportionate to parent window (and center vertically) */

        Top = 1 << (3), /**< value from the top (and center horizontally) */
        Bottom = 1 << (4), /**< value from the bottom (and center horizontally) */
        VStretch = Top | Bottom, /**< stretch vertically proportionate to parent window (and center horizontally) */

        Stretch = HStretch | VStretch, /**< stretch proportionate to parent window */
        Default = Left | Top, /**< default value (value from left and top) */

        HRelative = 1 << (5),
        VRelative = 1 << (6),
        Relative = HRelative | VRelative,

        LeftTop = Left | Top,
        LeftBottom = Left | Bottom,
        RightTop = Right | Top,
        RightBottom = Right | Bottom
    }

    /// <summary>
    /// This interface abstracts how the timelines display images.
    /// </summary>
    public interface ITextDisplay : IDisposable
    {
        /// <summary>
        /// Set the image for this IImageDisplay.
        /// </summary>
        /// <param name="image"></param>
        void setText(String text);

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

        String FontName { get; set; }

        int FontHeight { get; set; }

        TextualAlignment TextAlign { get; set; }
    }
}

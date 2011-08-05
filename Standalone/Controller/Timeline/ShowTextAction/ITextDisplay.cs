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
        LeftTop = 0,
        LeftBottom = 1,
        RightTop = 2,
        RightBottom = 3,
        TopCenter = 4,
        BottomCenter = 5,
        LeftCenter = 6,
        RightCenter = 7,
        Center = 8
    }

    /// <summary>
    /// This interface abstracts how the timelines display images.
    /// </summary>
    public interface ITextDisplay : IDisposable
    {
        event EventDelegate<ITextDisplay, String> TextEdited;

        /// <summary>
        /// Add a color to the text.
        /// </summary>
        /// <param name="color">The color to add.</param>
        /// <returns>A new string formatted with the color info.</returns>
        String addColorToSelectedText(Color color);

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

        bool Editable { get; set; }

        Vector3 ScenePoint { get; set; }

        bool PositionOnScenePoint { get; set; }
    }
}

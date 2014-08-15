using FreeImageAPI;
using OgreWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    public static class FreeImageBitmapExtensions
    {
        /// <summary>
        /// Create a pixel box for this FreeImageBitmap, note that ogre will populate the pixel box upside down, so you will need to flip the image
        /// afterward. Also you must Dispose the PixelBox returned by this function.
        /// </summary>
        /// /// <param name="bitmap">This object.</param>
        /// <param name="format">The format of the pixel box.</param>
        /// <returns>A PixelBox with the given format for the FreeImageBitmap.</returns>
        unsafe public static PixelBox createPixelBox(this FreeImageBitmap bitmap, OgreWrapper.PixelFormat format)
        {
            return new PixelBox(0, 0, bitmap.Width, bitmap.Height, format, bitmap.GetScanlinePointer(0).ToPointer());
        }

        /// <summary>
        /// Copy the given RenderTarget with the specified format.
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="renderTarget"></param>
        /// <param name="format"></param>
        public static void copyFromRenderTarget(this FreeImageBitmap bitmap, RenderTarget renderTarget, OgreWrapper.PixelFormat format)
        {
            using (PixelBox pixelBox = bitmap.createPixelBox(format))
            {
                renderTarget.copyContentsToMemory(pixelBox, RenderTarget.FrameBuffer.FB_AUTO);
            }
            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
        }
    }
}

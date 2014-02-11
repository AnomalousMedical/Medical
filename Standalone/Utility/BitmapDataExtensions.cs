using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace Medical
{
    public static class BitmapDataExtensions
    {
        /// <summary>
        /// Set the alpha of a given BitmapData object. This assumes that 
        /// </summary>
        /// <param name="bmpData"></param>
        /// <param name="alpha"></param>
        public static void SetAlpha(BitmapData bmpData, byte alpha)
        {
            var line = bmpData.Scan0;
            var eof = line + bmpData.Height * bmpData.Stride;
            while (line != eof)
            {
                var pixelAlpha = line + 3;
                var eol = pixelAlpha + bmpData.Width * 4;
                while (pixelAlpha != eol)
                {
                    System.Runtime.InteropServices.Marshal.WriteByte(pixelAlpha, alpha);
                    pixelAlpha += 4;
                }
                line += bmpData.Stride;
            }
        }
    }
}

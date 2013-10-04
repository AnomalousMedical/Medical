using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace Anomalous.Medical.StoreManager.Util
{
    public class LogoUtil
    {
        /// <summary>
        /// Save a resized image maintaining aspect ratio, the width and height specify the max width and max height.
        /// </summary>
        public static void SaveResizedImage(Bitmap source, Stream stream, int width, int height)
        {
            SaveResizedImage(source, stream, ref width, ref height);
        }

        /// <summary>
        /// Save a resized image maintaining aspect ratio, the width and height specify the max width and max height.
        /// </summary>
        public static void SaveResizedImage(Bitmap source, Stream stream, ref int width, ref int height)
        {
            if (source.Width > source.Height)
            {
                float ratio = (float)width / (float)source.Width;
                width = (int)(source.Width * ratio);
                height = (int)(source.Height * ratio);
            }
            else if(source.Width < source.Height)
            {
                float ratio = (float)height / (float)source.Height;
                width = (int)(source.Width * ratio);
                height = (int)(source.Height * ratio);
            }

            using (Bitmap resized = new Bitmap(width, height))
            {
                using (Graphics graphics = Graphics.FromImage(resized))
                {
                    graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    graphics.DrawImage(source, 0, 0, resized.Width, resized.Height);
                }
                resized.Save(stream, ImageFormat.Png);
            }
        }
    }
}

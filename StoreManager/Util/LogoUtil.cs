using FreeImageAPI;
using System;
using System.Collections.Generic;
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
        public static void SaveResizedImage(FreeImageBitmap source, Stream stream, int width, int height)
        {
            SaveResizedImage(source, stream, ref width, ref height);
        }

        /// <summary>
        /// Save a resized image maintaining aspect ratio, the width and height specify the max width and max height.
        /// </summary>
        public static void SaveResizedImage(FreeImageBitmap source, Stream stream, ref int width, ref int height)
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

            using(FreeImageBitmap resized = new FreeImageBitmap(source))
            {
                resized.Rescale(width, height, FREE_IMAGE_FILTER.FILTER_LANCZOS3);
                resized.Save(stream, FREE_IMAGE_FORMAT.FIF_PNG);
            }
        }
    }
}

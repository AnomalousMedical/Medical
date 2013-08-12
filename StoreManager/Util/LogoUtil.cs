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
        public static void SaveResizedImage(Bitmap source, Stream stream, int width, int height)
        {
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

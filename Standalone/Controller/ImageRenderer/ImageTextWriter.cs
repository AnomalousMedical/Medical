using FreeImageAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    public interface ImageTextWriter
    {
        /// <summary>
        /// Write the specified text string to the given image at the given size.
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="p"></param>
        void writeText(FreeImageBitmap bitmap, string p, int fontSize);
    }
}

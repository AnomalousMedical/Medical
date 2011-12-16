using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    class ImageRenderException : Exception
    {
        public ImageRenderException(String message)
            :base(message)
        {

        }
    }
}

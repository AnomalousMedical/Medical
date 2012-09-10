using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    class ImageException : Exception
    {
        public ImageException(String message)
            : base(message)
        {

        }
    }
}

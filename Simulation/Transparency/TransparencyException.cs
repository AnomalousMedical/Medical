using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    class TransparencyException : Exception
    {
        public TransparencyException(String message)
            :base(message)
        {

        }
    }
}

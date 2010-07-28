using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller
{
    class MDIException : Exception
    {
        public MDIException(String message)
            :base(message)
        {
        }
    }
}

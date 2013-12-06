using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class UpdateException : Exception
    {
        public UpdateException() { }
        public UpdateException(string message) : base(message) { }
        public UpdateException(string message, Exception inner) : base(message, inner) { }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class TimelineException : Exception
    {
        public TimelineException() { }
        public TimelineException(string message) : base(message) { }
        public TimelineException(string message, Exception inner) : base(message, inner) { }
    }
}

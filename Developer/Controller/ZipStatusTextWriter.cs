using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Logging;

namespace Developer
{
    class ZipStatusTextWriter : TextWriter
    {
        private StringBuilder stringBuilder = new StringBuilder();

        public override void WriteLine(string value)
        {
            Log.Info(value);
        }

        public override void WriteLine(string format, object arg0)
        {
            Log.Info(format, arg0);
        }

        public override void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            Log.Info(format, arg0, arg1, arg2);
        }

        public override void WriteLine(string format, object arg0, object arg1)
        {
            Log.Info(format, arg0, arg1);
        }

        public override void WriteLine(string format, params object[] arg)
        {
            Log.Info(format, arg);
        }

        public override Encoding Encoding
        {
            get { return Encoding.Default; }
        }
    }
}

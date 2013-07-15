using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;

namespace Medical
{
    class Mime : IDisposable
    {
        public Mime()
        {
            Headers = new NameValueCollection();
        }

        public void Dispose()
        {
            Data.DisposeIfNotNull();
        }

        public Stream Data { get; set; }

        public NameValueCollection Headers { get; private set; }

        public byte[] Header { get; private set; }

        public long GenerateHeader(String boundary)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("--");
            sb.Append(boundary);
            sb.AppendLine();
            foreach (string key in Headers.AllKeys)
            {
                sb.Append(key);
                sb.Append(": ");
                sb.AppendLine(Headers[key]);
            }
            sb.AppendLine();

            Header = Encoding.UTF8.GetBytes(sb.ToString());

            return Header.Length + Data.Length + 2;
        }
    }
}

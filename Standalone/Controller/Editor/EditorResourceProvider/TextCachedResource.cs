using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Medical
{
    public abstract class TextCachedResource : CachedResource
    {
        public TextCachedResource(String file, String cachedString = null)
            :base(file)
        {
            CachedString = cachedString;
        }

        public override Stream openStream()
        {
            byte[] preamble = Encoding.Unicode.GetPreamble();
            byte[] data = Encoding.Unicode.GetBytes(CachedString);
            MemoryStream ms = new MemoryStream(preamble.Length + data.Length);
            ms.Write(preamble, 0, preamble.Length);
            ms.Write(data, 0, data.Length);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        public String CachedString { get; set; }
    }
}

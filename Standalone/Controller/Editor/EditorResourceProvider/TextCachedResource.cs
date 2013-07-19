using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Medical
{
    public abstract class TextCachedResource : CachedResource
    {
        public TextCachedResource(String file, Encoding textEncoding, String cachedString = null)
            :base(file)
        {
            CachedString = cachedString;
            TextEncoding = textEncoding;
        }

        public override Stream openStream()
        {
            //This is a bit hacky, but since the StreamReader defaults to utf-8 we don't actually have to include
            //the preamble in those strings. This fixes problems with other libraries (like libRocket) that cannot
            //deal with the preamble, but expect UTF-8.
            if (TextEncoding == Encoding.UTF8)
            {
                return new MemoryStream(TextEncoding.GetBytes(CachedString));
            }
            else
            {
                byte[] preamble = TextEncoding.GetPreamble();
                byte[] data = TextEncoding.GetBytes(CachedString);
                MemoryStream ms = new MemoryStream(preamble.Length + data.Length);
                ms.Write(preamble, 0, preamble.Length);
                ms.Write(data, 0, data.Length);
                ms.Seek(0, SeekOrigin.Begin);
                return ms;
            }
        }

        public String CachedString { get; set; }

        public Encoding TextEncoding { get; set; }
    }
}

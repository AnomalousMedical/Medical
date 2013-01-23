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
            return new MemoryStream(UTF32Encoding.Default.GetBytes(CachedString));
        }

        public String CachedString { get; set; }
    }
}

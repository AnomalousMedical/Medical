using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Medical
{
    public class ResourceProviderTextCachedResource : TextCachedResource
    {
        EditorResourceProvider resourceProvider;

        public ResourceProviderTextCachedResource(String file, Encoding textEncoding, String text, EditorResourceProvider resourceProvider)
            :base(file, textEncoding, text)
        {
            this.resourceProvider = resourceProvider;
        }

        public override void save()
        {
            //Save the file
            using (StreamWriter streamWriter = new StreamWriter(resourceProvider.openWriteStream(File)))
            {
                streamWriter.Write(CachedString);
            }
            resourceProvider.ResourceCache.closeResource(File);
        }
    }
}

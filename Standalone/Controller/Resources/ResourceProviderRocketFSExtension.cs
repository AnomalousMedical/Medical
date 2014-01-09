using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libRocketPlugin;
using System.IO;

namespace Medical
{
    public class ResourceProviderRocketFSExtension : RocketFileSystemExtension
    {
        private ResourceProvider resourceProvider;
        private String fixedBackingLocation;

        public ResourceProviderRocketFSExtension(ResourceProvider resourceProvider)
        {
            this.resourceProvider = resourceProvider;
            refresh();
        }

        public bool canOpenFile(string file)
        {
            if(file.StartsWith(fixedBackingLocation))
            {
                return resourceProvider.exists(file.Substring(fixedBackingLocation.Length));
            }
            return false;
        }

        public Stream openFile(string file)
        {
            if (file.StartsWith(fixedBackingLocation))
            {
                return resourceProvider.openFile(file.Substring(fixedBackingLocation.Length));
            }
            return null;
        }

        internal void refresh()
        {
            fixedBackingLocation = resourceProvider.BackingLocation.Replace('\\', '/');
            if (!fixedBackingLocation.EndsWith("/"))
            {
                fixedBackingLocation += "/";
            }
        }
    }
}

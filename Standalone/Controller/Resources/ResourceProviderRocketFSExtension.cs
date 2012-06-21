using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libRocketPlugin;
using System.IO;

namespace Medical
{
    class ResourceProviderRocketFSExtension : RocketFileSystemExtension
    {
        private ResourceProvider resourceProvider;

        public ResourceProviderRocketFSExtension(ResourceProvider resourceProvider)
        {
            this.resourceProvider = resourceProvider;
        }

        public bool canOpenFile(string file)
        {
            return resourceProvider.exists(file);
        }

        public Stream openFile(string file)
        {
            return resourceProvider.openFile(file);
        }
    }
}

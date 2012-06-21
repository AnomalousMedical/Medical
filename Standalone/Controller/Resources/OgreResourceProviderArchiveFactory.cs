using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgreWrapper;

namespace Medical
{
    class OgreResourceProviderArchiveFactory : OgreManagedArchiveFactory
    {
        public const String Name = "OgreResourceProviderArchive";
        private static Dictionary<String, ResourceProvider> resourceProviders = new Dictionary<string, ResourceProvider>();

        public static void AddResourceProvider(String name, ResourceProvider resourceProvider)
        {
            resourceProviders.Add(name, resourceProvider);
        }

        public OgreResourceProviderArchiveFactory()
            : base(Name)
        {

        }

        protected override OgreManagedArchive doCreateInstance(string name)
        {
            OgreResourceProviderArchive archive = new OgreResourceProviderArchive(name, Name, resourceProviders[name]);
            resourceProviders.Remove(name);
            return archive;
        }
    }
}

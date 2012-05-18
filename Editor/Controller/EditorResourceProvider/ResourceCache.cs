using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class ResourceCache
    {
        private Dictionary<String, CachedResource> cachedResources = new Dictionary<string, CachedResource>();

        public ResourceCache()
        {

        }

        public void add(CachedResource resource)
        {
            String file = resource.File.ToLowerInvariant();
            if (!cachedResources.ContainsKey(file))
            {
                cachedResources.Add(file, resource);
            }
            else
            {
                cachedResources[file] = resource;
            }
        }

        public void remove(String filename)
        {
            cachedResources.Remove(filename.ToLowerInvariant());
        }

        public void closeResource(String filename)
        {
            filename = filename.ToLowerInvariant();
            CachedResource resource;
            if (cachedResources.TryGetValue(filename, out resource) && resource.AllowClose)
            {
                cachedResources.Remove(filename);
            }
        }

        public void clear()
        {
            cachedResources.Clear();
        }

        public CachedResource this[String filename]
        {
            get
            {
                CachedResource resource;
                cachedResources.TryGetValue(filename.ToLowerInvariant(), out resource);
                return resource;
            }
        }

        public IEnumerable<CachedResource> Resources
        {
            get
            {
                return cachedResources.Values;
            }
        }
    }
}

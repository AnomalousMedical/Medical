using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class ResourceCache
    {
        private Dictionary<String, CachedResource> cachedResources = new Dictionary<string, CachedResource>();

        public delegate void ResourceCacheEvent(ResourceCache resourceCache, CachedResource resource);

        public event ResourceCacheEvent CachedResourceAdded;
        public event ResourceCacheEvent CachedResourceRemoved;

        public ResourceCache()
        {

        }

        public void add(CachedResource resource)
        {
            String file = fixPath(resource.File);
            if (!cachedResources.ContainsKey(file))
            {
                cachedResources.Add(file, resource);
            }
            else
            {
                cachedResources[file] = resource;
            }
            if (CachedResourceAdded != null)
            {
                CachedResourceAdded.Invoke(this, resource);
            }
        }

        public void closeResource(String filename)
        {
            filename = fixPath(filename);
            CachedResource resource;
            if (cachedResources.TryGetValue(filename, out resource) && resource.AllowClose)
            {
                cachedResources.Remove(filename);
                if (CachedResourceRemoved != null)
                {
                    CachedResourceRemoved.Invoke(this, resource);
                }
            }
        }

        public void clear()
        {
            cachedResources.Clear();
        }

        public LinkedList<CachedResource> getResourcesInDirectory(String directory)
        {
            LinkedList<CachedResource> directoryResources = new LinkedList<CachedResource>();
            foreach (CachedResource cachedResource in cachedResources.Values)
            {
                if (cachedResource.isInDirectory(directory))
                {
                    directoryResources.AddLast(cachedResource);
                }
            }
            return directoryResources;
        }

        public void forceCloseResourceFile(String filename)
        {
            CachedResource cachedResource = this[filename];
            if (cachedResource != null)
            {
                cachedResource.AllowClose = true;
                closeResource(cachedResource.File);
            }
        }

        public void forceCloseResourcesInDirectroy(String filename)
        {
            foreach (CachedResource cachedResource in getResourcesInDirectory(filename))
            {
                cachedResource.AllowClose = true;
                closeResource(cachedResource.File);
            }
        }

        public CachedResource this[String filename]
        {
            get
            {
                CachedResource resource;
                cachedResources.TryGetValue(fixPath(filename), out resource);
                return resource;
            }
        }

        public IEnumerable<CachedResource> Resources
        {
            get
            {
                return new LinkedList<CachedResource>(cachedResources.Values);
            }
        }

        public IEnumerable<String> OpenFiles
        {
            get
            {
                return cachedResources.Values.Select(cachedResource => 
                {
                    return cachedResource.File;
                });
            }
        }

        public int Count
        {
            get
            {
                return cachedResources.Count;
            }
        }

        public static String fixPath(String path)
        {
            return path.ToLowerInvariant().Replace('\\', '/');
        }
    }
}

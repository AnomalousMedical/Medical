using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Medical
{
    /// <summary>
    /// An abstract base class for a cacheable resource.
    /// </summary>
    public abstract class CachedResource
    {
        public CachedResource(String file)
        {
            this.File = file;
            AllowClose = true;
        }

        public abstract Stream openStream();

        public abstract void save();

        public bool isSameFile(string filePath)
        {
            return ResourceCache.fixPath(filePath) == ResourceCache.fixPath(File);
        }

        public bool isInDirectory(String directory)
        {
            return ResourceCache.fixPath(File).StartsWith(ResourceCache.fixPath(directory));
        }

        public String File { get; private set; }

        public bool AllowClose { get; set; }
    }
}

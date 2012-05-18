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

        public String File { get; private set; }

        public bool AllowClose { get; set; }
    }
}

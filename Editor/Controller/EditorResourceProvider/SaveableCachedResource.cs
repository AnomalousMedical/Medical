using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using Engine.Saving;

namespace Medical
{
    /// <summary>
    /// A class that can easily cache objects that extend saveable. This has
    /// first class support in the EditorResourceProvider, so if you use caching
    /// and the openSaveable function in EditorResourceProvider the saveables
    /// will automatically come from the cache if needed and do not need any
    /// further management.
    /// </summary>
    class SaveableCachedResource : CachedResource
    {
        public SaveableCachedResource(String file, Saveable saveable)
            :base(file)
        {
            this.Saveable = saveable;
        }

        public Saveable Saveable { get; private set; }

        public override Stream openStream()
        {
            return new SaveableObjectStream(Saveable);
        }
    }
}

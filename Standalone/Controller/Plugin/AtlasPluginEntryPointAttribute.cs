using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public abstract class AtlasPluginEntryPointAttribute : Attribute
    {
        public AtlasPluginEntryPointAttribute()
        {
            
        }

        public abstract void createPlugin(StandaloneController standaloneController);
    }
}

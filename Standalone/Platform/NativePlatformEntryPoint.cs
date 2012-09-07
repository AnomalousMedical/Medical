using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

[assembly: Medical.NativePlatformEntryPoint()]

namespace Medical
{
    class NativePlatformEntryPoint : PluginEntryPointAttribute
    {
        public override void createPluginInterfaces(PluginManager pluginManager)
        {
            pluginManager.addPlugin(new NativePlatformPlugin());
        }
    }
}

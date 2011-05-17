using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

[assembly: Medical.WxPlatformEntryPoint()]

namespace Medical
{
    class WxPlatformEntryPoint : PluginEntryPointAttribute
    {
        public override void createPluginInterfaces(PluginManager pluginManager)
        {
            pluginManager.addPlugin(new WxPlatformPlugin());
        }
    }
}

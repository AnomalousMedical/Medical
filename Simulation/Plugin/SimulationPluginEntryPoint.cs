using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

[assembly: Medical.SimulationPluginEntryPoint()]

namespace Medical
{
    class SimulationPluginEntryPoint : PluginEntryPointAttribute
    {
        public override void createPluginInterfaces(PluginManager pluginManager)
        {
            pluginManager.addPlugin(new SimulationPlugin());
        }
    }
}

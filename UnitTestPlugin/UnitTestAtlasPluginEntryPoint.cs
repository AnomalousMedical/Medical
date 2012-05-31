using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical;

[assembly: UnitTestPlugin.UnitTestAtlasPluginEntryPoint()]

namespace UnitTestPlugin
{
    class UnitTestAtlasPluginEntryPoint : AtlasPluginEntryPointAttribute
    {
        public override void createPlugin(StandaloneController standaloneController)
        {
            standaloneController.AtlasPluginManager.addPlugin(new UnitTestAtlasPlugin());
        }
    }
}

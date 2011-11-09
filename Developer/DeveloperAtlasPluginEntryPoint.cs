using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical;

[assembly: Developer.DeveloperAtlasPluginEntryPoint()]

namespace Developer
{
    class DeveloperAtlasPluginEntryPoint : AtlasPluginEntryPointAttribute
    {
        public override void createPlugin(StandaloneController standaloneController)
        {
            standaloneController.AtlasPluginManager.addPlugin(new DeveloperAtlasPlugin(standaloneController));
        }
    }
}

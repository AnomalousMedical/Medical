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
            if (standaloneController.App.LicenseManager.allowFeature(8))
            {
                standaloneController.AtlasPluginManager.addPlugin(new DeveloperAtlasPlugin(standaloneController));
            }
        }
    }
}

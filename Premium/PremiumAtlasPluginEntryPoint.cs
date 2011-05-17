using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[assembly: Medical.PremiumAtlasPluginEntryPoint()]

namespace Medical
{
    class PremiumAtlasPluginEntryPoint : AtlasPluginEntryPointAttribute
    {
        public override void createPlugin(StandaloneController standaloneController)
        {
            standaloneController.AtlasPluginManager.addPlugin(new PremiumBodyAtlasPlugin(standaloneController));
        }
    }
}

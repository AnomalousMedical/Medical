using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical;

namespace DentalSim
{
    class DentalSimAtlasPluginEntryPoint : AtlasPluginEntryPointAttribute
    {
        public override void createPlugin(StandaloneController standaloneController)
        {
            if (standaloneController.App.LicenseManager.allowFeature(2))
            {
                standaloneController.AtlasPluginManager.addPlugin(new DentalSimPlugin());
            }
        }
    }
}

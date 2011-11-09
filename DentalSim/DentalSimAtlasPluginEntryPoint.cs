using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical;
using DentalSim;

[assembly: DentalSimAtlasPluginEntryPoint()]

namespace DentalSim
{
    class DentalSimAtlasPluginEntryPoint : AtlasPluginEntryPointAttribute
    {
        public override void createPlugin(StandaloneController standaloneController)
        {
            standaloneController.AtlasPluginManager.addPlugin(new DentalSimPlugin());
        }
    }
}

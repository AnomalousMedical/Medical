using Medical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: Anomalous.Medical.StoreManager.StoreManagerPluginEntryPoint()]

namespace Anomalous.Medical.StoreManager
{
    class StoreManagerPluginEntryPoint : AtlasPluginEntryPointAttribute
    {
        public override void createPlugin(StandaloneController standaloneController)
        {
            standaloneController.AtlasPluginManager.addPlugin(new StoreManagerPlugin());
        }
    }
}

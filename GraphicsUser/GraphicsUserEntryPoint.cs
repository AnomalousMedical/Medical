using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical;

[assembly: GraphicsUser.GraphicsUserEntryPoint()]

namespace GraphicsUser
{
    class GraphicsUserEntryPoint : AtlasPluginEntryPointAttribute
    {
        public override void createPlugin(StandaloneController standaloneController)
        {
            if (standaloneController.App.LicenseManager.allowFeature(10))
            {
                standaloneController.AtlasPluginManager.addPlugin(new GraphicsUserPlugin());
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[assembly: Medical.PremiumAtlasPluginEntryPoint()]

namespace Medical
{
    enum Features
    {
        Premium = 1
    }

    class PremiumAtlasPluginEntryPoint : AtlasPluginEntryPointAttribute
    {
        public override void createPlugin(StandaloneController standaloneController)
        {
            if (standaloneController.App.LicenseManager.allowFeature((int)Features.Premium))
            {
                standaloneController.AtlasPluginManager.addPlugin(new PremiumBodyAtlasPlugin(standaloneController));
            }
        }
    }
}

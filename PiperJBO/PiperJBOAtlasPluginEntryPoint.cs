using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;

[assembly: Medical.PiperJBOAtlasPluginEntryPoint()]

namespace Medical
{
    class PiperJBOAtlasPluginEntryPoint : AtlasPluginEntryPointAttribute
    {
        public override void createPlugin(StandaloneController standaloneController)
        {
            standaloneController.AtlasPluginManager.addPlugin(new PiperJBOAtlasPlugin(standaloneController.App.LicenseManager));
        }
    }
}

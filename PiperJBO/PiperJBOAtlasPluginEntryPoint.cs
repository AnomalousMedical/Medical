using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;

[assembly: Medical.PiperJBOAtlasPluginEntryPoint()]

namespace Medical
{
    enum FeatureCodes
    {
        PiperJBOClinical = 3,
        PiperJBOImaging = 4
    }

    class PiperJBOAtlasPluginEntryPoint : AtlasPluginEntryPointAttribute
    {
        public override void createPlugin(StandaloneController standaloneController)
        {
            if (standaloneController.App.LicenseManager.allowFeature((int)FeatureCodes.PiperJBOClinical) || standaloneController.App.LicenseManager.allowFeature((int)FeatureCodes.PiperJBOImaging))
            {
                standaloneController.AtlasPluginManager.addPlugin(new PiperJBOAtlasPlugin(standaloneController.App.LicenseManager));
            }
        }
    }
}

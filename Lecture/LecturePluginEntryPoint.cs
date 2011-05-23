using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[assembly: Medical.LecturePluginEntryPoint()]

namespace Medical
{
    enum FeatureCodes
    {
        LecturePlugin = 2
    }

    class LecturePluginEntryPoint : AtlasPluginEntryPointAttribute
    {
        public override void createPlugin(StandaloneController standaloneController)
        {
            if (standaloneController.App.LicenseManager.allowFeature((int)FeatureCodes.LecturePlugin))
            {
                standaloneController.AtlasPluginManager.addPlugin(new LecturePlugin());
            }
        }
    }
}

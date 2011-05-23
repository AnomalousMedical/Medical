using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[assembly: Medical.EditorAtlasPluginEntryPoint()]

namespace Medical
{
    enum FeatureCodes
    {
        EditorPlugin = 2
    }

    class EditorAtlasPluginEntryPoint : AtlasPluginEntryPointAttribute
    {
        public override void createPlugin(StandaloneController standaloneController)
        {
            if (standaloneController.App.LicenseManager.allowFeature((int)FeatureCodes.EditorPlugin))
            {
                standaloneController.AtlasPluginManager.addPlugin(new EditorPlugin());
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[assembly: Medical.EditorAtlasPluginEntryPoint()]

namespace Medical
{
    class EditorAtlasPluginEntryPoint : AtlasPluginEntryPointAttribute
    {
        public override void createPlugin(StandaloneController standaloneController)
        {
            standaloneController.AtlasPluginManager.addPlugin(new EditorPlugin());
        }
    }
}

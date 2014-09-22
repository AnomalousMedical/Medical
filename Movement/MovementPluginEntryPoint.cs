using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[assembly: Medical.MovementPluginEntryPoint()]

namespace Medical
{
    class MovementPluginEntryPoint : AtlasPluginEntryPointAttribute
    {
        public override void createPlugin(StandaloneController standaloneController)
        {
            standaloneController.AtlasPluginManager.addPlugin(new MovementBodyAtlasPlugin());
        }
    }
}

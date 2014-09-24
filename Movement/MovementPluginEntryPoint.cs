using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[assembly: Medical.Movement.MovementPluginEntryPoint()]

namespace Medical.Movement
{
    class MovementPluginEntryPoint : AtlasPluginEntryPointAttribute
    {
        public override void createPlugin(StandaloneController standaloneController)
        {
            standaloneController.AtlasPluginManager.addPlugin(new MovementBodyAtlasPlugin());
        }
    }
}

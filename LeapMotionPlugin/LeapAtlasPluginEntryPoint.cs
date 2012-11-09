using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical;

[assembly: LeapMotionPlugin.LeapAtlasPluginEntryPoint()]

namespace LeapMotionPlugin
{
    class LeapAtlasPluginEntryPoint : AtlasPluginEntryPointAttribute
    {
        public override void createPlugin(StandaloneController standaloneController)
        {
            standaloneController.AtlasPluginManager.addPlugin(new LeapAtlasPlugin(standaloneController));
        }
    }
}

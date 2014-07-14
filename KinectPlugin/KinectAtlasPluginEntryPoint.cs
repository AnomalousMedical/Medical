using Medical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: KinectPlugin.KinectAtlasPluginEntryPoint()]

namespace KinectPlugin
{
    class KinectAtlasPluginEntryPoint : AtlasPluginEntryPointAttribute
    {
        public override void createPlugin(StandaloneController standaloneController)
        {
            standaloneController.AtlasPluginManager.addPlugin(new KinectAtlasPlugin());
        }
    }
}

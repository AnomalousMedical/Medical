using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[assembly: Medical.LecturePluginEntryPoint()]

namespace Medical
{
    class LecturePluginEntryPoint : AtlasPluginEntryPointAttribute
    {
        public override void createPlugin(StandaloneController standaloneController)
        {
            standaloneController.AtlasPluginManager.addPlugin(new LecturePlugin());
        }
    }
}

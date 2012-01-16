using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical;

[assembly: LectureBuilder.LectureBuilderEntryPoint()]

namespace LectureBuilder
{
    class LectureBuilderEntryPoint : AtlasPluginEntryPointAttribute
    {
        public override void createPlugin(StandaloneController standaloneController)
        {
            standaloneController.AtlasPluginManager.addPlugin(new LectureBuilderPlugin());
        }
    }
}

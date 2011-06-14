using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical;

[assembly: ExamExample.ExamExampleAtlasPluginEntryPoint()]

namespace ExamExample
{
    class ExamExampleAtlasPluginEntryPoint : AtlasPluginEntryPointAttribute
    {
        public override void createPlugin(StandaloneController standaloneController)
        {
            standaloneController.AtlasPluginManager.addPlugin(new ExamExampleAtlasPlugin(standaloneController));
        }
    }
}

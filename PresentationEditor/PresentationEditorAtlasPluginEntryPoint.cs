using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical;

[assembly: PresentationEditor.PresentationEditorAtlasPluginEntryPoint()]

namespace PresentationEditor
{
    class PresentationEditorAtlasPluginEntryPoint : AtlasPluginEntryPointAttribute
    {
        public override void createPlugin(StandaloneController standaloneController)
        {
            standaloneController.AtlasPluginManager.addPlugin(new PresentationEditorPlugin());
        }
    }
}

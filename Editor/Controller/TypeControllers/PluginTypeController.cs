using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;

namespace Medical
{
    class PluginTypeController : EditorTypeController
    {
        private DDAtlasPluginEditor editor;

        public PluginTypeController(DDAtlasPluginEditor editor)
            :base(".ddp")
        {
            this.editor = editor;
        }

        public override void openFile(string fullPath)
        {
            editor.loadPlugin(fullPath);
            if (!editor.Visible)
            {
                editor.open(false);
            }
            editor.activateExtensionActions();
            editor.bringToFront();
        }
    }
}

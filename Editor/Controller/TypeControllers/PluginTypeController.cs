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
        {
            this.editor = editor;
        }

        public bool canOpenFile(string extension)
        {
            return extension == ".ddp";
        }

        public void openFile(string fullPath)
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

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
        private EditorController editorController;

        public PluginTypeController(DDAtlasPluginEditor editor, EditorController editorController)
            :base(".ddp")
        {
            this.editor = editor;
            this.editorController = editorController;
        }

        public override void openFile(string file)
        {
            editor.loadPlugin(editorController.ResourceProvider.getFullFilePath(file));
            if (!editor.Visible)
            {
                editor.open(false);
            }
            editor.activateExtensionActions();
            editor.bringToFront();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;

namespace Medical
{
    class MvcTypeController : EditorTypeController
    {
        private GenericEditor editor;
        private EditorController editorController;

        public MvcTypeController(GenericEditor editor, EditorController editorController)
            :base(".mvc")
        {
            this.editor = editor;
            this.editorController = editorController; 
        }

        public override void openFile(string file)
        {
            editor.load(editorController.ResourceProvider.getFullFilePath(file));
            if (!editor.Visible)
            {
                editor.open(false);
            }
            editor.activateExtensionActions();
            editor.bringToFront();
        }
    }
}

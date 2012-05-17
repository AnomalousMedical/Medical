using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;

namespace Medical
{
    class RmlTypeController : EditorTypeController
    {
        private RmlViewer editor;
        private EditorController editorController;

        public RmlTypeController(RmlViewer editor, EditorController editorController)
            :base(".rml")
        {
            this.editor = editor;
            this.editorController = editorController;
        }

        public override void openFile(string file)
        {
            editor.changeDocument(editorController.ResourceProvider.getFullFilePath(file));
            if (!editor.Visible)
            {
                editor.open(false);
            }
            editor.activateExtensionActions();
            editor.bringToFront();
        }
    }
}

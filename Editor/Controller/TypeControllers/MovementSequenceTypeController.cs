using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;

namespace Medical
{
    class MovementSequenceTypeController : EditorTypeController
    {
        private MovementSequenceEditor editor;
        private EditorController editorController;

        public MovementSequenceTypeController(MovementSequenceEditor editor, EditorController editorController)
            :base(".seq")
        {
            this.editor = editor;
            this.editorController = editorController;
        }

        public override void openFile(string file)
        {
            editor.openSequence(editorController.ResourceProvider.getFullFilePath(file));
            if (!editor.Visible)
            {
                editor.open(false);
            }
            editor.activateExtensionActions();
            editor.bringToFront();
        }
    }
}

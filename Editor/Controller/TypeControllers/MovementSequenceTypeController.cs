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

        public MovementSequenceTypeController(MovementSequenceEditor editor)
            :base(".seq")
        {
            this.editor = editor;
        }

        public override void openFile(string fullPath)
        {
            editor.openSequence(fullPath);
            if (!editor.Visible)
            {
                editor.open(false);
            }
            editor.activateExtensionActions();
            editor.bringToFront();
        }
    }
}

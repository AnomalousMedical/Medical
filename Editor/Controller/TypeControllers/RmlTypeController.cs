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

        public RmlTypeController(RmlViewer editor)
            :base(".rml")
        {
            this.editor = editor;
        }

        public override void openFile(string fullPath)
        {
            editor.changeDocument(fullPath);
            if (!editor.Visible)
            {
                editor.open(false);
            }
            editor.activateExtensionActions();
            editor.bringToFront();
        }
    }
}

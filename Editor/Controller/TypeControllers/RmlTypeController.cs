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
        {
            this.editor = editor;
        }

        public bool canOpenFile(string extension)
        {
            return extension == ".rml";
        }

        public void openFile(string fullPath)
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

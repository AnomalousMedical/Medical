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

        public MvcTypeController(GenericEditor editor)
        {
            this.editor = editor;
        }

        public bool canOpenFile(string extension)
        {
            return extension == ".mvc";
        }

        public void openFile(string fullPath)
        {
            editor.load(fullPath);
            if (!editor.Visible)
            {
                editor.open(false);
            }
            editor.activateExtensionActions();
            editor.bringToFront();
        }
    }
}

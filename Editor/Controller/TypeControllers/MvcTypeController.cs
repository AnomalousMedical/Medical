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
            :base(".mvc")
        {
            this.editor = editor;
        }

        public override void openFile(string fullPath)
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

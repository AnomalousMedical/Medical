using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;

namespace Medical
{
    class TimelineTypeController : EditorTypeController
    {
        private TimelineEditor editor;

        public TimelineTypeController(TimelineEditor editor)
        {
            this.editor = editor;
        }

        public bool canOpenFile(string extension)
        {
            return extension == ".tl";
        }

        public void openFile(string fullPath)
        {
            editor.loadTimeline(fullPath);
            if (!editor.Visible)
            {
                editor.open(false);
            }
            editor.activateExtensionActions();
            editor.bringToFront();
        }
    }
}

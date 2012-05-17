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
            :base(".tl")
        {
            this.editor = editor;
        }

        public override void openFile(string fullPath)
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

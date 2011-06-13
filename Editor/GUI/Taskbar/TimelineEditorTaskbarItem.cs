using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class TimelineEditorTaskbarItem : TaskbarItem
    {
        private TimelineProperties timelineProperties;
        private TimelineObjectEditor timelineObjectEditor;
        private TimelineFileExplorer timelineFileExplorer;

        public TimelineEditorTaskbarItem(TimelineProperties timelineProperties, TimelineObjectEditor timelineObjectEditor, TimelineFileExplorer timelineFileExplorer)
            :base("Timeline", "TimelineEditorIcon")
        {
            this.timelineProperties = timelineProperties;
            this.timelineObjectEditor = timelineObjectEditor;
            this.timelineFileExplorer = timelineFileExplorer;
        }

        public override void clicked(Widget source, EventArgs e)
        {
            timelineFileExplorer.Visible = timelineObjectEditor.Visible = timelineProperties.Visible = !timelineProperties.Visible;
        }
    }
}

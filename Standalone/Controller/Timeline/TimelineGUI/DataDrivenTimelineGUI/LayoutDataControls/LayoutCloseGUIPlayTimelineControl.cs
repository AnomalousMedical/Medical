using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Logging;

namespace Medical.LayoutDataControls
{
    class LayoutCloseGUIPlayTimelineControl : LayoutWidgetDataControl
    {
        private Button button;
        private CloseGUIPlayTimelineField field;
        private DataDrivenTimelineGUI gui;

        public LayoutCloseGUIPlayTimelineControl(Button button, DataDrivenTimelineGUI gui, CloseGUIPlayTimelineField field)
        {
            this.button = button;
            button.MouseButtonClick += new MyGUIEvent(button_MouseButtonClick);

            this.field = field;
            this.gui = gui;
        }

        void button_MouseButtonClick(Widget source, EventArgs e)
        {
            if (field.Timeline != null)
            {
                gui.closeAndPlayTimeline(field.Timeline);
            }
            else
            {
                Log.Warning("Could not play menu timelines from button '{0}' in timeline '{1}' because none are defined. Returning to main GUI.", field.Name, gui.TimelineFile);
            }
        }

        public override void captureData(DataDrivenExamSection examSection)
        {
            
        }

        public override void displayData(DataDrivenExamSection examSection)
        {
            
        }
    }
}

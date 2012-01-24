using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logging;
using MyGUIPlugin;

namespace Medical.LayoutDataControls
{
    class LayoutPlayExampleButton : LayoutWidgetDataControl
    {
        private Button button;
        private PlayExampleDataField playExampleField;
        private DataDrivenTimelineGUI gui;

        public LayoutPlayExampleButton(Button button, DataDrivenTimelineGUI gui, PlayExampleDataField playExampleField)
        {
            this.button = button;
            button.MouseButtonClick += new MyGUIEvent(button_MouseButtonClick);

            this.playExampleField = playExampleField;
            this.gui = gui;
        }

        void button_MouseButtonClick(Widget source, EventArgs e)
        {
            if (playExampleField.Timeline != null)
            {
                gui.playExampleTimeline(playExampleField.Timeline);
            }
            else
            {
                Log.Warning("Could not play menu timelines from button '{0}' in timeline '{1}' because none are defined. Returning to main GUI.", playExampleField.Name, gui.TimelineFile);
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

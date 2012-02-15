using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.LayoutDataControls
{
    class LayoutDoActionsDataControl : LayoutWidgetDataControl
    {
        private Button button;
        private DoActionsDataField dataField;
        private DataDrivenTimelineGUI gui;

        public LayoutDoActionsDataControl(Button button, DataDrivenTimelineGUI gui, DoActionsDataField dataField)
        {
            this.button = button;
            button.MouseButtonClick += new MyGUIEvent(button_MouseButtonClick);

            this.dataField = dataField;
            this.gui = gui;
        }

        void button_MouseButtonClick(Widget source, EventArgs e)
        {
            dataField.doActions(gui);
        }

        public override void captureData(DataDrivenExamSection examSection)
        {
            
        }

        public override void displayData(DataDrivenExamSection examSection)
        {
            
        }
    }
}

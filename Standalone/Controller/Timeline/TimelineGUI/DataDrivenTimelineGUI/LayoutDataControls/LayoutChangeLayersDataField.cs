using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.LayoutDataControls
{
    class LayoutChangeLayersDataField : LayoutWidgetDataControl
    {
        private Button button;
        private ChangeLayersDataField dataField;
        private DataDrivenTimelineGUI gui;

        public LayoutChangeLayersDataField(Button button, DataDrivenTimelineGUI gui, ChangeLayersDataField dataField)
        {
            this.button = button;
            button.MouseButtonClick += new MyGUIEvent(button_MouseButtonClick);

            this.dataField = dataField;
            this.gui = gui;
        }

        void button_MouseButtonClick(Widget source, EventArgs e)
        {
            gui.applyLayers(dataField.Layers);
        }

        public override void captureData(DataDrivenExamSection examSection)
        {
            
        }

        public override void displayData(DataDrivenExamSection examSection)
        {
            
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.LayoutDataControls
{
    class LayoutMoveCameraChangeLayersDataField : LayoutWidgetDataControl
    {
        private Button button;
        private MoveCameraChangeLayersDataField dataField;
        private DataDrivenTimelineGUI gui;

        public LayoutMoveCameraChangeLayersDataField(Button button, DataDrivenTimelineGUI gui, MoveCameraChangeLayersDataField dataField)
        {
            this.button = button;
            button.MouseButtonClick += new MyGUIEvent(button_MouseButtonClick);

            this.dataField = dataField;
            this.gui = gui;
        }

        void button_MouseButtonClick(Widget source, EventArgs e)
        {
            gui.applyCameraPosition(dataField.CameraPosition);
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

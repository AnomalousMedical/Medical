using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.LayoutDataControls
{
    class LayoutChangeMedicalStateDataControl : LayoutWidgetDataControl
    {
        private Button button;
        private ChangeMedicalStateDataField dataField;
        private DataDrivenTimelineGUI gui;

        public LayoutChangeMedicalStateDataControl(Widget parentWidget, DataDrivenTimelineGUI gui, ChangeMedicalStateDataField dataField)
        {
            button = (Button)parentWidget.createWidgetT("Button", "Button", 0, 0, 100, DataDrivenTimelineGUI.BUTTON_HEIGHT, Align.Default, "");
            button.MouseButtonClick += new MyGUIEvent(button_MouseButtonClick);
            button.Caption = dataField.Name;
            button.ForwardMouseWheelToParent = true;

            this.dataField = dataField;
            this.gui = gui;
        }

        void button_MouseButtonClick(Widget source, EventArgs e)
        {
            gui.applyPresetState(dataField.PresetState, dataField.Duration);
        }

        public override void captureData(DataDrivenExamSection examSection)
        {
            
        }

        public override void displayData(DataDrivenExamSection examSection)
        {
            
        }
    }
}

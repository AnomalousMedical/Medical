using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical
{
    class ChangeMedicalStateDataControl : DataControl
    {
        private Button button;
        private ChangeMedicalStateDataField dataField;
        private DataDrivenTimelineGUI gui;

        public ChangeMedicalStateDataControl(Widget parentWidget, DataDrivenTimelineGUI gui, ChangeMedicalStateDataField dataField)
        {
            button = (Button)parentWidget.createWidgetT("Button", "Button", 0, 0, 100, DataDrivenTimelineGUI.BUTTON_HEIGHT, Align.Default, "");
            button.MouseButtonClick += new MyGUIEvent(button_MouseButtonClick);
            button.Caption = dataField.Name;
            button.ForwardMouseWheelToParent = true;

            this.dataField = dataField;
            this.gui = gui;
        }

        public override void Dispose()
        {
            Gui.Instance.destroyWidget(button);
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

        public override void bringToFront()
        {
            
        }

        public override void setAlpha(float alpha)
        {

        }

        public override void layout()
        {
            button.setPosition((int)Location.x, (int)Location.y);
            button.setSize((int)WorkingSize.Width, (int)WorkingSize.Height);
            Height = button.Height;
        }

        public override Size2 DesiredSize
        {
            get
            {
                return new Size2(button.Width, button.Height);
            }
        }

        public override bool Visible
        {
            get
            {
                return button.Visible;
            }
            set
            {
                button.Visible = value;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using Logging;

namespace Medical
{
    public class MoveCameraDataControl : DataControl
    {
        private Button button;
        private MoveCameraDataField dataField;
        private DataDrivenTimelineGUI gui;

        public MoveCameraDataControl(Widget parentWidget, DataDrivenTimelineGUI gui, MoveCameraDataField dataField)
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
            gui.applyCameraPosition(dataField.CameraPosition);
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

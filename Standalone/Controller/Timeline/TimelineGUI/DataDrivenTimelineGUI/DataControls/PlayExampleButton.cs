using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using MyGUIPlugin;
using Logging;

namespace Medical
{
    class PlayExampleButton : DataControl
    {
        private Button button;
        private PlayExampleDataField playExampleField;
        private DataDrivenTimelineGUI gui;

        public PlayExampleButton(Widget parentWidget, DataDrivenTimelineGUI gui, PlayExampleDataField playExampleField)
        {
            button = (Button)parentWidget.createWidgetT("Button", "Button", 0, 0, 100, DataDrivenTimelineGUI.BUTTON_HEIGHT, Align.Default, "");
            button.MouseButtonClick += new MyGUIEvent(button_MouseButtonClick);
            button.Caption = playExampleField.Name;

            this.playExampleField = playExampleField;
            this.gui = gui;
        }

        public override void Dispose()
        {
            Gui.Instance.destroyWidget(button);
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

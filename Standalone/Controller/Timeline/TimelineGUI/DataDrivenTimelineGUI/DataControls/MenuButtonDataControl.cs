using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using MyGUIPlugin;
using Logging;
using Engine.Editing;

namespace Medical
{
    class MenuButtonDataControl : DataControl
    {
        private Button button;
        private MenuItemField menuItemField;
        private DataDrivenTimelineGUI gui;

        public MenuButtonDataControl(Widget parentWidget, DataDrivenTimelineGUI gui, MenuItemField menuItemField)
        {
            button = (Button)parentWidget.createWidgetT("Button", "Button", 0, 0, 100, DataDrivenTimelineGUI.BUTTON_HEIGHT, Align.Default, "");
            button.MouseButtonClick += new MyGUIEvent(button_MouseButtonClick);
            button.Caption = menuItemField.Name;

            this.menuItemField = menuItemField;
            this.gui = gui;
        }

        public override void Dispose()
        {
            Gui.Instance.destroyWidget(button);
        }

        void button_MouseButtonClick(Widget source, EventArgs e)
        {
            DataDrivenNavigationState dataNavState = menuItemField.createNavigationState(gui.TimelineFile);
            DataDrivenNavigationManager.Instance.pushNavigationState(dataNavState);
            dataNavState.configureGUI(gui);

            DataDrivenExamSection section = DataDrivenExamController.Instance.CurrentSection.getSection(menuItemField.Name);
            DataDrivenExamController.Instance.pushCurrentSection(section);
            if (dataNavState.CurrentTimeline != null)
            {
                gui.closeAndPlayTimeline(dataNavState.CurrentTimeline);
            }
            else
            {
                Log.Warning("Could not play menu timelines from button '{0}' in timeline '{1}' because none are defined. Returning to main GUI.", menuItemField.Name, gui.TimelineFile);
                gui.closeAndReturnToMainGUI();
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

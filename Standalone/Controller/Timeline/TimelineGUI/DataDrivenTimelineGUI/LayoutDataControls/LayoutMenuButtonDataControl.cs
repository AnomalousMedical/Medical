using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Logging;

namespace Medical.LayoutDataControls
{
    class LayoutMenuButtonDataControl : LayoutWidgetDataControl
    {
        private Button button;
        private MenuItemField menuItemField;
        private DataDrivenTimelineGUI gui;

        public LayoutMenuButtonDataControl(Button button, DataDrivenTimelineGUI gui, MenuItemField menuItemField)
        {
            this.button = button;
            button.MouseButtonClick += new MyGUIEvent(button_MouseButtonClick);
            this.menuItemField = menuItemField;
            this.gui = gui;
        }

        public override void captureData(DataDrivenExamSection examSection)
        {
            
        }

        public override void displayData(DataDrivenExamSection examSection)
        {
            
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
    }
}

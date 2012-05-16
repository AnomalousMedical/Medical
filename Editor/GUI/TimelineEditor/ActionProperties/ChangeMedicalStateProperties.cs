using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class ChangeMedicalStateProperties : TimelineDataPanel
    {
        private ChangeMedicalStateAction changeStateAction;

        public ChangeMedicalStateProperties(Widget parentWidget)
            : base(parentWidget, "Medical.GUI.TimelineEditor.ActionProperties.ChangeMedicalStateProperties.layout")
        {
            Button useCurrent = mainWidget.findWidget("UseCurrent") as Button;
            useCurrent.MouseButtonClick += new MyGUIEvent(useCurrent_MouseButtonClick);
        }

        public override void setCurrentData(TimelineData data)
        {
            changeStateAction = (ChangeMedicalStateAction)((TimelineActionData)data).Action;
        }

        void useCurrent_MouseButtonClick(Widget source, EventArgs e)
        {
            changeStateAction.capture();
        }
    }
}

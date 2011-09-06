using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class MusclePositionActionProperties : TimelineDataPanel
    {
        private MusclePositionAction playSequence;

        public MusclePositionActionProperties(Widget parentWidget)
            : base(parentWidget, "Medical.GUI.Timeline.ActionProperties.MusclePositionActionProperties.layout")
        {
            Button useCurrent = mainWidget.findWidget("UseCurrent") as Button;
            useCurrent.MouseButtonClick += new MyGUIEvent(useCurrent_MouseButtonClick);
        }

        public override void setCurrentData(TimelineData data)
        {
            playSequence = (MusclePositionAction)((TimelineActionData)data).Action;
        }

        void useCurrent_MouseButtonClick(Widget source, EventArgs e)
        {
            playSequence.capture();
        }
    }
}

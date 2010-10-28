using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Muscles;
using MyGUIPlugin;

namespace Medical.GUI
{
    class MovementKeyframeProperties : TimelineDataPanel
    {
        private MovementSequenceState movementSequenceState;

        public MovementKeyframeProperties(Widget parentWidget)
            : base(parentWidget, "Medical.GUI.MovementSequence.MovementKeyframeProperties.layout")
        {
            Button useCurrent = mainWidget.findWidget("UseCurrent") as Button;
            useCurrent.MouseButtonClick += new MyGUIEvent(useCurrent_MouseButtonClick);
        }

        public override void setCurrentData(TimelineData data)
        {
            movementSequenceState = ((MovementKeyframeData)data).KeyFrame;
        }

        void useCurrent_MouseButtonClick(Widget source, EventArgs e)
        {
            movementSequenceState.captureState();
        }
    }
}

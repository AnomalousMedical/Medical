using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Muscles;
using MyGUIPlugin;

namespace Medical.GUI
{
    class OffsetKeyframeProperties : TimelineDataPanel
    {
        private OffsetModifierKeyframe keyframe;

        public OffsetKeyframeProperties(Widget parentWidget)
            : base(parentWidget, "Medical.GUI.OffsetSequence.OffsetKeyframeProperties.layout")
        {
            Button useCurrent = mainWidget.findWidget("UseCurrent") as Button;
            useCurrent.MouseButtonClick += new MyGUIEvent(useCurrent_MouseButtonClick);
        }

        public override void setCurrentData(TimelineData data)
        {
            keyframe = ((OffsetKeyframeData)data).KeyFrame;
        }

        void useCurrent_MouseButtonClick(Widget source, EventArgs e)
        {
            //Capture
            //keyframe.captureState();
        }
    }
}

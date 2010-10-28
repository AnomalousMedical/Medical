using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class PlaySequenceProperties : TimelineDataPanel
    {
        private PlaySequenceAction playSequence;

        public PlaySequenceProperties(Widget parentWidget)
            :base(parentWidget, "Medical.GUI.Timeline.ActionProperties.PlaySequenceProperties.layout")
        {
            Button useCurrent = mainWidget.findWidget("UseCurrent") as Button;
            useCurrent.MouseButtonClick += new MyGUIEvent(useCurrent_MouseButtonClick);

            Button edit = mainWidget.findWidget("Edit") as Button;
            edit.MouseButtonClick += new MyGUIEvent(edit_MouseButtonClick);
        }

        public override void setCurrentData(TimelineData data)
        {
            playSequence = (PlaySequenceAction)((TimelineActionData)data).Action;
        }

        void edit_MouseButtonClick(Widget source, EventArgs e)
        {
            playSequence.edit();
        }

        void useCurrent_MouseButtonClick(Widget source, EventArgs e)
        {
            playSequence.capture();
        }
    }
}

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
        private CheckButton pauseOnStop;
        private bool allowSync = true;

        public PlaySequenceProperties(Widget parentWidget)
            :base(parentWidget, "Medical.GUI.Timeline.ActionProperties.PlaySequenceProperties.layout")
        {
            Button useCurrent = mainWidget.findWidget("UseCurrent") as Button;
            useCurrent.MouseButtonClick += new MyGUIEvent(useCurrent_MouseButtonClick);

            pauseOnStop = new CheckButton(mainWidget.findWidget("PauseOnStop") as Button);
            pauseOnStop.CheckedChanged += new MyGUIEvent(pauseOnStop_CheckedChanged);
        }

        public override void setCurrentData(TimelineData data)
        {
            allowSync = false;
            playSequence = (PlaySequenceAction)((TimelineActionData)data).Action;
            pauseOnStop.Checked = playSequence.PauseOnStop;
            allowSync = true;
        }

        void useCurrent_MouseButtonClick(Widget source, EventArgs e)
        {
            playSequence.capture();
        }

        void pauseOnStop_CheckedChanged(Widget source, EventArgs e)
        {
            if (allowSync)
            {
                playSequence.PauseOnStop = pauseOnStop.Checked;
            }
        }
    }
}

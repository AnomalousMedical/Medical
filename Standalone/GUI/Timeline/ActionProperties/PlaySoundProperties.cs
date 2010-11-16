using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class PlaySoundProperties : TimelineDataPanel
    {
        private PlaySoundAction playSound;
        private Edit soundFileEdit;

        public PlaySoundProperties(Widget parentWidget)
            :base(parentWidget, "Medical.GUI.Timeline.ActionProperties.PlaySoundProperties.layout")
        {
            soundFileEdit = mainWidget.findWidget("SoundFileEdit") as Edit;

            Button browseButton = mainWidget.findWidget("BrowseButton") as Button;
            browseButton.MouseButtonClick += new MyGUIEvent(browseButton_MouseButtonClick);
        }

        public override void setCurrentData(TimelineData data)
        {
            playSound = (PlaySoundAction)((TimelineActionData)data).Action;
            soundFileEdit.Caption = playSound.SoundFile;
        }

        void browseButton_MouseButtonClick(Widget source, EventArgs e)
        {
            
        }
    }
}

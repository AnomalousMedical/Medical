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
        private TimelineData timelineData;
        private EditBox soundFileEdit;

        public PlaySoundProperties(Widget parentWidget)
            :base(parentWidget, "Medical.GUI.TimelineEditor.ActionProperties.PlaySoundProperties.layout")
        {
            soundFileEdit = mainWidget.findWidget("SoundFileEdit") as EditBox;

            Button browseButton = mainWidget.findWidget("BrowseButton") as Button;
            browseButton.MouseButtonClick += new MyGUIEvent(browseButton_MouseButtonClick);
        }

        public override void setCurrentData(TimelineData data)
        {
            timelineData = data;
            playSound = (PlaySoundAction)((TimelineActionData)data).Action;
            soundFileEdit.Caption = playSound.SoundFile;
        }

        void browseButton_MouseButtonClick(Widget source, EventArgs e)
        {
            playSound.TimelineController.promptForFile("*.ogg", fileChosen);
        }

        void fileChosen(String filename)
        {
            playSound.SoundFile = filename;
            soundFileEdit.Caption = playSound.SoundFile;
            timelineData.Duration = (float)playSound.TimelineController.getSoundDuration(filename);
        }
    }
}

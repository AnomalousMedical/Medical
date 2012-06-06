using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Editing;
using Medical.Editor;

namespace Medical.GUI
{
    class PlaySoundProperties : TimelineDataPanel
    {
        private PlaySoundAction playSound;
        private TimelineData timelineData;
        private EditBox soundFileEdit;
        private MedicalUICallback uiCallback;

        public PlaySoundProperties(Widget parentWidget, MedicalUICallback uiCallback)
            :base(parentWidget, "Medical.GUI.TimelineEditor.ActionProperties.PlaySoundProperties.layout")
        {
            this.uiCallback = uiCallback;

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
            Browser oggBrowser = BrowserWindowController.createFileBrowser("*.ogg");
            uiCallback.showBrowser<String>("Choose Ogg File", oggBrowser, fileChosen);
        }

        bool fileChosen(String file, ref string errorPrompt)
        {
            playSound.SoundFile = file;
            soundFileEdit.Caption = playSound.SoundFile;
            timelineData.Duration = (float)playSound.TimelineController.getSoundDuration(file);
            return true;
        }
    }
}

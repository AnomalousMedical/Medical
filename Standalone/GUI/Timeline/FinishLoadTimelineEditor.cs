using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class FinishLoadTimelineEditor
    {
        private Edit fileText;
        private CheckButton continueCheck;
        private Button chooseButton;
        private TimelineFileBrowserDialog fileBrowser;

        public FinishLoadTimelineEditor(Widget window, TimelineFileBrowserDialog fileBrowser)
        {
            this.fileBrowser = fileBrowser;
            fileText = window.findWidget("FileText") as Edit;
            continueCheck = new CheckButton(window.findWidget("ContinueCheck") as Button);
            chooseButton = window.findWidget("ChooseButton") as Button;
            chooseButton.MouseButtonClick += new MyGUIEvent(chooseButton_MouseButtonClick);
            Enabled = false;
        }

        public void setProperties(LoadAnotherTimeline loadAnotherTimelineAction)
        {
            fileText.Caption = loadAnotherTimelineAction.TargetTimeline;
            continueCheck.Checked = loadAnotherTimelineAction.ShowContinuePrompt;
        }

        public bool Enabled
        {
            get
            {
                return fileText.Enabled;
            }
            set
            {
                fileText.Enabled = value;
                continueCheck.Enabled = value;
                chooseButton.Enabled = value;
            }
        }

        public String File
        {
            get
            {
                return fileText.Caption;
            }
            set
            {
                fileText.Caption = value;
            }
        }

        public bool ShowContinuePrompt
        {
            get
            {
                return continueCheck.Checked;
            }
            set
            {
                continueCheck.Checked = value;
            }
        }

        private void chooseButton_MouseButtonClick(Widget source, EventArgs e)
        {
            fileBrowser.Position = new Vector2(source.AbsoluteLeft, source.AbsoluteTop);
            fileBrowser.ensureVisible();
            fileBrowser.promptForFile("*.tl", fileChosen);
        }

        private void fileChosen(String filename)
        {
            fileText.Caption = filename;
        }
    }
}

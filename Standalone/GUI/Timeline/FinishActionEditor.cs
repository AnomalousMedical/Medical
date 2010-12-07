using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class FinishActionEditor : Dialog
    {
        private ButtonGroup actionGroup = new ButtonGroup();

        private Button doNothingButton;
        private Button loadTimelineButton;
        private Button askQuestionButton;

        private Edit fileText;
        private CheckButton continueCheck;
        private Button chooseButton;

        private Timeline currentTimeline;
        private LoadAnotherTimeline loadAnotherTimelineAction;

        private TimelineController timelineController;
        private TimelineFileBrowserDialog fileBrowser;

        public FinishActionEditor(TimelineController timelineController, TimelineFileBrowserDialog fileBrowser)
            :base("Medical.GUI.Timeline.FinishActionEditor.layout")
        {
            this.timelineController = timelineController;
            this.fileBrowser = fileBrowser;

            doNothingButton = window.findWidget("DoNothingRadio") as Button;
            actionGroup.addButton(doNothingButton);
            loadTimelineButton = window.findWidget("LoadTimelineRadio") as Button;
            actionGroup.addButton(loadTimelineButton);
            askQuestionButton = window.findWidget("AskQuestionRadio") as Button;
            actionGroup.addButton(askQuestionButton);
            actionGroup.SelectedButtonChanged += new EventHandler(actionGroup_SelectedButtonChanged);

            fileText = window.findWidget("FileText") as Edit;
            continueCheck = new CheckButton(window.findWidget("ContinueCheck") as Button);
            chooseButton = window.findWidget("ChooseButton") as Button;
            chooseButton.MouseButtonClick += new MyGUIEvent(chooseButton_MouseButtonClick);
            setLoadTimelinePropertiesEnabled(false);

            Button applyButton = window.findWidget("ApplyButton") as Button;
            applyButton.MouseButtonClick += new MyGUIEvent(applyButton_MouseButtonClick);
            Button cancelButton = window.findWidget("CancelButton") as Button;
            cancelButton.MouseButtonClick += new MyGUIEvent(cancelButton_MouseButtonClick);
        }

        public Timeline CurrentTimeline
        {
            get
            {
                return currentTimeline;
            }
            set
            {
                currentTimeline = value;
            }
        }

        protected override void onShown(EventArgs args)
        {
            base.onShown(args);
            loadAnotherTimelineAction = null;
            foreach (TimelineInstantAction action in currentTimeline.PostActions)
            {
                if (action is LoadAnotherTimeline)
                {
                    loadAnotherTimelineAction = action as LoadAnotherTimeline;
                    break;
                }
            }

            if (loadAnotherTimelineAction != null)
            {
                actionGroup.SelectedButton = loadTimelineButton;
                fileText.Caption = loadAnotherTimelineAction.TargetTimeline;
                continueCheck.Checked = loadAnotherTimelineAction.ShowContinuePrompt;
            }
            else
            {
                actionGroup.SelectedButton = doNothingButton;
            }
        }

        void cancelButton_MouseButtonClick(Widget source, EventArgs e)
        {
            this.close();
        }

        void applyButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (actionGroup.SelectedButton == doNothingButton)
            {
                if (loadAnotherTimelineAction != null)
                {
                    currentTimeline.removePostAction(loadAnotherTimelineAction);
                    loadAnotherTimelineAction = null;
                }
                this.close();
            }
            else if (actionGroup.SelectedButton == loadTimelineButton)
            {
                if (timelineController.listResourceFiles(fileText.Caption).Length > 0)
                {
                    if (loadAnotherTimelineAction == null)
                    {
                        loadAnotherTimelineAction = new LoadAnotherTimeline();
                        currentTimeline.addPostAction(loadAnotherTimelineAction);
                    }
                    loadAnotherTimelineAction.TargetTimeline = fileText.Caption;
                    loadAnotherTimelineAction.ShowContinuePrompt = continueCheck.Checked;
                    this.close();
                }
                else
                {
                    MessageBox.show("Please select a valid timeline to transition to.", "Error", MessageBoxStyle.Ok | MessageBoxStyle.IconError);
                }
            }
            else if (actionGroup.SelectedButton == askQuestionButton)
            {
                ShowPromptAction showPrompt = new ShowPromptAction();
                PromptQuestion question = new PromptQuestion("Does this test question work?");
                question.addAnswer(new PromptAnswer("Yes"));
                question.addAnswer(new PromptAnswer("No"));
                showPrompt.addQuestion(question);
                currentTimeline.addPostAction(showPrompt);
            }
        }

        void actionGroup_SelectedButtonChanged(object sender, EventArgs e)
        {
            if (actionGroup.SelectedButton == doNothingButton)
            {
                setLoadTimelinePropertiesEnabled(false);
            }
            else if (actionGroup.SelectedButton == loadTimelineButton)
            {
                setLoadTimelinePropertiesEnabled(true);
            }
            else if (actionGroup.SelectedButton == askQuestionButton)
            {
                setLoadTimelinePropertiesEnabled(false);
            }
        }

        private void setLoadTimelinePropertiesEnabled(bool enabled)
        {
            fileText.Enabled = enabled;
            continueCheck.Enabled = enabled;
            chooseButton.Enabled = enabled;
        }

        void chooseButton_MouseButtonClick(Widget source, EventArgs e)
        {
            fileBrowser.Position = new Vector2(source.AbsoluteLeft, source.AbsoluteTop);
            fileBrowser.ensureVisible();
            fileBrowser.promptForFile("*.tl", fileChosen);
        }

        void fileChosen(String filename)
        {
            fileText.Caption = filename;
        }
    }
}

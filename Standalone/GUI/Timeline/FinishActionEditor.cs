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

        private FinishLoadTimelineEditor loadTimelineEditor;

        private Timeline currentTimeline;
        private LoadAnotherTimeline loadAnotherTimelineAction;
        private ShowPromptAction showPromptAction;

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

            loadTimelineEditor = new FinishLoadTimelineEditor(window, fileBrowser);

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
            showPromptAction = null;
            foreach (TimelineInstantAction action in currentTimeline.PostActions)
            {
                if (action is LoadAnotherTimeline)
                {
                    loadAnotherTimelineAction = action as LoadAnotherTimeline;
                    break;
                }
                if (action is ShowPromptAction)
                {
                    showPromptAction = action as ShowPromptAction;
                }
            }

            if (loadAnotherTimelineAction != null)
            {
                actionGroup.SelectedButton = loadTimelineButton;
                loadTimelineEditor.setProperties(loadAnotherTimelineAction);
            }
            else if (showPromptAction != null)
            {
                actionGroup.SelectedButton = askQuestionButton;
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
                currentTimeline.clearPostActions();
                showPromptAction = null;
                loadAnotherTimelineAction = null;
                this.close();
            }
            else if (actionGroup.SelectedButton == loadTimelineButton)
            {
                String timelineFileName = loadTimelineEditor.File;
                if (timelineController.listResourceFiles(timelineFileName).Length > 0)
                {
                    currentTimeline.clearPostActions();

                    if (loadAnotherTimelineAction == null)
                    {
                        loadAnotherTimelineAction = new LoadAnotherTimeline();
                    }
                    loadAnotherTimelineAction.TargetTimeline = timelineFileName;
                    loadAnotherTimelineAction.ShowContinuePrompt = loadTimelineEditor.ShowContinuePrompt;
                    currentTimeline.addPostAction(loadAnotherTimelineAction);
                    this.close();
                }
                else
                {
                    MessageBox.show("Please select a valid timeline to transition to.", "Error", MessageBoxStyle.Ok | MessageBoxStyle.IconError);
                }
            }
            else if (actionGroup.SelectedButton == askQuestionButton)
            {
                currentTimeline.clearPostActions();

                //Temp create prompt
                ShowPromptAction showPrompt = new ShowPromptAction();
                PromptQuestion question = new PromptQuestion("Does this test question work?");
                PromptAnswer yes = new PromptAnswer("Yes");
                yes.Action = new PromptLoadTimelineAction("yes.tl");
                question.addAnswer(yes);
                PromptAnswer no = new PromptAnswer("No");
                no.Action = new PromptLoadTimelineAction("no.tl");
                question.addAnswer(no);
                showPrompt.addQuestion(question);
                currentTimeline.addPostAction(showPrompt);
            }
        }

        void actionGroup_SelectedButtonChanged(object sender, EventArgs e)
        {
            if (actionGroup.SelectedButton == doNothingButton)
            {
                loadTimelineEditor.Enabled = false;
            }
            else if (actionGroup.SelectedButton == loadTimelineButton)
            {
                loadTimelineEditor.Enabled = true;
            }
            else if (actionGroup.SelectedButton == askQuestionButton)
            {
                loadTimelineEditor.Enabled = false;
            }
        }
    }
}

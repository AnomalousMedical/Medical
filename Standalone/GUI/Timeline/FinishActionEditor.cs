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
        private QuestionEditor questionEditor;
        private Button questionEditorButton;

        private Timeline currentTimeline;

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
            questionEditor = new QuestionEditor(fileBrowser);

            questionEditorButton = window.findWidget("QuestionEditorButton") as Button;
            questionEditorButton.MouseButtonClick += new MyGUIEvent(questionEditorButton_MouseButtonClick);
            questionEditorButton.Enabled = false;

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

            actionGroup.SelectedButton = doNothingButton;

            foreach (TimelineInstantAction action in currentTimeline.PostActions)
            {
                if (action is LoadAnotherTimeline)
                {
                    loadTimelineEditor.setProperties(action as LoadAnotherTimeline);
                    actionGroup.SelectedButton = loadTimelineButton;
                    break;
                }
                if (action is ShowPromptAction)
                {
                    ShowPromptAction showPromptAction = action as ShowPromptAction;
                    foreach(PromptQuestion question in showPromptAction.Questions)
                    {
                        questionEditor.Question = question;
                        break;
                    }
                    actionGroup.SelectedButton = askQuestionButton;
                    break;
                }
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
                this.close();
            }
            else if (actionGroup.SelectedButton == loadTimelineButton)
            {
                String timelineFileName = loadTimelineEditor.File;
                if (timelineController.listResourceFiles(timelineFileName).Length > 0)
                {
                    currentTimeline.clearPostActions();
                    currentTimeline.addPostAction(loadTimelineEditor.createAction());
                    this.close();
                }
                else
                {
                    MessageBox.show("Please select a valid timeline to transition to.", "Error", MessageBoxStyle.Ok | MessageBoxStyle.IconError);
                }
            }
            else if (actionGroup.SelectedButton == askQuestionButton)
            {
                if (questionEditor.Question != null)
                {
                    currentTimeline.clearPostActions();

                    ShowPromptAction showPrompt = new ShowPromptAction();
                    PromptQuestion question = questionEditor.Question;
                    showPrompt.addQuestion(question);
                    currentTimeline.addPostAction(showPrompt);
                    this.close();
                }
                else
                {
                    MessageBox.show("There is no question currently defined. Please open the question editor and create one.", "Error", MessageBoxStyle.Ok | MessageBoxStyle.IconError);
                }
            }

            questionEditor.clear();
        }

        void actionGroup_SelectedButtonChanged(object sender, EventArgs e)
        {
            if (actionGroup.SelectedButton == doNothingButton)
            {
                loadTimelineEditor.Enabled = false;
                questionEditorButton.Enabled = false;
            }
            else if (actionGroup.SelectedButton == loadTimelineButton)
            {
                loadTimelineEditor.Enabled = true;
                questionEditorButton.Enabled = false;
            }
            else if (actionGroup.SelectedButton == askQuestionButton)
            {
                loadTimelineEditor.Enabled = false;
                questionEditorButton.Enabled = true;
            }
        }

        void questionEditorButton_MouseButtonClick(Widget source, EventArgs e)
        {
            questionEditor.open(true);
        }
    }
}

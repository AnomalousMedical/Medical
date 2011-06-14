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
        private Button repeatActionButton;
        private Button showGUIButton;

        private FinishLoadTimelineEditor loadTimelineEditor;
        private Button questionEditorButton;
        private Button guiEditorButton;
        private ShowGUIEditor guiEditor;

        private Timeline currentTimeline;

        private TimelineController timelineController;
        private TimelineFileBrowserDialog fileBrowser;

        public FinishActionEditor(TimelineController timelineController, TimelineFileBrowserDialog fileBrowser, GUIManager guiManager)
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
            repeatActionButton = window.findWidget("RepeatAction") as Button;
            actionGroup.addButton(repeatActionButton);
            showGUIButton = window.findWidget("ShowGUIRadio") as Button;
            actionGroup.addButton(showGUIButton);
            actionGroup.SelectedButtonChanged += new EventHandler(actionGroup_SelectedButtonChanged);

            loadTimelineEditor = new FinishLoadTimelineEditor(window, fileBrowser);

            guiEditor = new ShowGUIEditor(fileBrowser, timelineController);
            guiManager.addManagedDialog(guiEditor);

            questionEditorButton = window.findWidget("QuestionEditorButton") as Button;
            questionEditorButton.MouseButtonClick += new MyGUIEvent(questionEditorButton_MouseButtonClick);
            questionEditorButton.Enabled = false;

            guiEditorButton = window.findWidget("GUIEditorButton") as Button;
            guiEditorButton.MouseButtonClick += new MyGUIEvent(guiEditorButton_MouseButtonClick);
            guiEditorButton.Enabled = false;

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
                    //ShowPromptAction showPromptAction = action as ShowPromptAction;
                    //foreach(PromptQuestion question in showPromptAction.Questions)
                    //{
                    //    questionEditor.Question = question;
                    //    break;
                    //}
                    //questionEditor.SoundFile = showPromptAction.SoundFile;
                    //actionGroup.SelectedButton = askQuestionButton;
                    //break;
                }
                if (action is RepeatPreviousPostActions)
                {
                    actionGroup.SelectedButton = repeatActionButton;
                }
                if (action is ShowTimelineGUIAction)
                {
                    actionGroup.SelectedButton = showGUIButton;
                    guiEditor.setProperties((ShowTimelineGUIAction)action);
                }
            }
        }

        void cancelButton_MouseButtonClick(Widget source, EventArgs e)
        {
            guiEditor.clear();
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
                //if (questionEditor.Question != null)
                //{
                //    currentTimeline.clearPostActions();

                //    ShowPromptAction showPrompt = new ShowPromptAction();
                //    PromptQuestion question = questionEditor.Question;
                //    showPrompt.addQuestion(question);
                //    currentTimeline.addPostAction(showPrompt);
                //    showPrompt.SoundFile = questionEditor.SoundFile;
                //    this.close();
                //}
                //else
                //{
                //    MessageBox.show("There is no question currently defined. Please open the question editor and create one.", "Error", MessageBoxStyle.Ok | MessageBoxStyle.IconError);
                //}
            }
            else if (actionGroup.SelectedButton == repeatActionButton)
            {
                currentTimeline.clearPostActions();
                currentTimeline.addPostAction(new RepeatPreviousPostActions());
                this.close();
            }
            else if (actionGroup.SelectedButton == showGUIButton)
            {
                currentTimeline.clearPostActions();
                currentTimeline.addPostAction(guiEditor.createAction());
                this.close();
            }

            //questionEditor.clear();
            guiEditor.clear();
        }

        void actionGroup_SelectedButtonChanged(object sender, EventArgs e)
        {
            loadTimelineEditor.Enabled = actionGroup.SelectedButton == loadTimelineButton;
            questionEditorButton.Enabled = actionGroup.SelectedButton == askQuestionButton;
            guiEditorButton.Enabled = actionGroup.SelectedButton == showGUIButton;
        }

        void questionEditorButton_MouseButtonClick(Widget source, EventArgs e)
        {
            //questionEditor.open(true);
        }

        void guiEditorButton_MouseButtonClick(Widget source, EventArgs e)
        {
            guiEditor.open(true);
        }
    }
}

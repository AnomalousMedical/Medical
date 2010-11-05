using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using System.IO;

namespace Medical.GUI
{
    class FinishActionEditor : Dialog
    {
        private ButtonGroup actionGroup = new ButtonGroup();

        private Button doNothingButton;
        private Button loadTimelineButton;
        private Button askQuestionButton;

        private ComboBox timelineFileCombo;
        private CheckButton continueCheck;
        private Button importButton;

        private Timeline currentTimeline;
        private LoadAnotherTimeline loadAnotherTimelineAction;

        private TimelineController timelineController;

        public FinishActionEditor(TimelineController timelineController)
            :base("Medical.GUI.Timeline.FinishActionEditor.layout")
        {
            this.timelineController = timelineController;

            doNothingButton = window.findWidget("DoNothingRadio") as Button;
            actionGroup.addButton(doNothingButton);
            loadTimelineButton = window.findWidget("LoadTimelineRadio") as Button;
            actionGroup.addButton(loadTimelineButton);
            askQuestionButton = window.findWidget("AskQuestionRadio") as Button;
            actionGroup.addButton(askQuestionButton);
            actionGroup.SelectedButtonChanged += new EventHandler(actionGroup_SelectedButtonChanged);

            timelineFileCombo = window.findWidget("TimelineFileCombo") as ComboBox;
            continueCheck = new CheckButton(window.findWidget("ContinueCheck") as Button);
            importButton = window.findWidget("ImportButton") as Button;
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

            timelineFileCombo.removeAllItems();
            String[] files = timelineController.listResourceFiles("*.tl");
            foreach (String file in files)
            {
                timelineFileCombo.addItem(Path.GetFileNameWithoutExtension(file), Path.GetFileName(file));
            }

            if (loadAnotherTimelineAction != null)
            {
                actionGroup.SelectedButton = loadTimelineButton;
                uint index = timelineFileCombo.findItemIndexWith(Path.GetFileNameWithoutExtension(loadAnotherTimelineAction.TargetTimeline));
                if (index == uint.MaxValue)
                {
                    MessageBox.show("The transition target timeline does not exist. Please choose another.", "Error", MessageBoxStyle.Ok | MessageBoxStyle.IconError);
                    if (timelineFileCombo.ItemCount > 0)
                    {
                        timelineFileCombo.SelectedIndex = 0;
                    }
                }
                else
                {
                    timelineFileCombo.SelectedIndex = index;
                }
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
                uint comboIndex = timelineFileCombo.SelectedIndex;
                if (comboIndex != uint.MaxValue)
                {
                    if (loadAnotherTimelineAction == null)
                    {
                        loadAnotherTimelineAction = new LoadAnotherTimeline();
                        currentTimeline.addPostAction(loadAnotherTimelineAction);
                    }
                    loadAnotherTimelineAction.TargetTimeline = timelineFileCombo.getItemDataAt(timelineFileCombo.SelectedIndex).ToString();
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
            timelineFileCombo.Enabled = enabled;
            continueCheck.Enabled = enabled;
            importButton.Enabled = enabled;
        }
    }
}

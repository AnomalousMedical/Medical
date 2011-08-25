using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using Engine;
using MyGUIPlugin;
using Logging;

namespace Medical
{
    public class DataDrivenTimelineGUI : GenericTimelineGUI<DataDrivenTimelineGUIData>
    {
        private DataControl topLevelDataControl;
        private DataDrivenExamSection guiSection;

        public DataDrivenTimelineGUI()
            :base("Medical.Controller.Timeline.TimelineGUI.DataDrivenTimelineGUI.DataDrivenTimelineGUI.layout")
        {
            
        }

        public override void Dispose()
        {
            topLevelDataControl.Dispose();
            base.Dispose();
        }

        protected override void onShown()
        {
            Vector2 startPos = new Vector2(0, 0);
            if (DataDrivenNavigationManager.Instance.Count == 0)
            {
                //Make a close button and exam if this was launched directly.
                Button previewCloseButton = (Button)widget.createWidgetT("Button", "Button", 0, 0, 100, 20, MyGUIPlugin.Align.Default, "");
                previewCloseButton.MouseButtonClick += new MyGUIEvent(previewCloseButton_MouseButtonClick);
                previewCloseButton.Caption = "Close";
                startPos.y = previewCloseButton.Bottom;

                if (!DataDrivenExamController.Instance.HasCurrentSection)
                {
                    DataDrivenExam exam = DataDrivenExamController.Instance.createOrRetrieveExam(SectionName);
                    DataDrivenExamController.Instance.pushCurrentSection(exam);
                    guiSection = exam;
                }
                else
                {
                    guiSection = DataDrivenExamController.Instance.CurrentSection;
                }
            }
            else
            {
                guiSection = DataDrivenExamController.Instance.CurrentSection.getSection(SectionName);

                int xLoc = 0;
                if (DataDrivenNavigationManager.Instance.Current.PreviousTimeline != null)
                {
                    Button previousButton = (Button)widget.createWidgetT("Button", "Button", xLoc, 0, widget.Width / 3, 20, MyGUIPlugin.Align.Default, "");
                    previousButton.MouseButtonClick += new MyGUIEvent(previousButton_MouseButtonClick);
                    previousButton.Caption = "Previous";
                    startPos.y = previousButton.Bottom;
                    xLoc += previousButton.Width;
                }
                if (DataDrivenNavigationManager.Instance.Current.NextTimeline != null)
                {
                    Button nextButton = (Button)widget.createWidgetT("Button", "Button", xLoc, 0, widget.Width / 3, 20, MyGUIPlugin.Align.Default, "");
                    nextButton.MouseButtonClick += new MyGUIEvent(nextButton_MouseButtonClick);
                    nextButton.Caption = "Next";
                    startPos.y = nextButton.Bottom;
                    xLoc += nextButton.Width;
                }
                if (DataDrivenNavigationManager.Instance.Current.MenuTimeline != null)
                {
                    Button finishButton = (Button)widget.createWidgetT("Button", "Button", xLoc, 0, widget.Width / 3, 20, MyGUIPlugin.Align.Default, "");
                    finishButton.MouseButtonClick += new MyGUIEvent(finishButton_MouseButtonClick);
                    finishButton.Caption = "Finish";
                    startPos.y = finishButton.Bottom;
                    xLoc += finishButton.Width;
                }
            }

            topLevelDataControl = GUIData.createControls(widget, this);
            topLevelDataControl.displayData(guiSection);
            topLevelDataControl.WorkingSize = new Size2(widget.Width, widget.Height);
            topLevelDataControl.Location = startPos;
            topLevelDataControl.layout();
        }

        public String SectionName
        {
            get
            {
                if (DataDrivenNavigationManager.Instance.Count > 0)
                {
                    if (TimelineFile != null)
                    {
                        DataDrivenNavigationState navState = DataDrivenNavigationManager.Instance.Current;
                        String timeline = navState.getNameForTimeline(TimelineFile);
                        if (timeline != null)
                        {
                            return timeline;
                        }
                        Log.Warning("Could not find name for timeline {0}. Is it part of the menu that opened this timeline?", TimelineFile);
                        return TimelineFile;
                    }
                    else
                    {
                        return "Not Saved Yet";
                    }
                }
                else
                {
                    return TimelineFile != null ? TimelineFile : "Not Saved Yet";
                }
            }
        }

        protected override void closing()
        {
            topLevelDataControl.captureData(guiSection);
        }

        void previewCloseButton_MouseButtonClick(Widget source, EventArgs e)
        {
            this.closeAndReturnToMainGUI();
            DataDrivenExamController.Instance.saveAndClear();
        }

        protected override void navigationBarChangedTimelines(String timeline)
        {
            DataDrivenNavigationManager.Instance.Current.CurrentTimeline = timeline;
        }

        void previousButton_MouseButtonClick(Widget source, EventArgs e)
        {
            String timeline = DataDrivenNavigationManager.Instance.Current.PreviousTimeline;
            DataDrivenNavigationManager.Instance.Current.CurrentTimeline = timeline;
            this.closeAndPlayTimeline(timeline);
        }

        void nextButton_MouseButtonClick(Widget source, EventArgs e)
        {
            String timeline = DataDrivenNavigationManager.Instance.Current.NextTimeline;
            DataDrivenNavigationManager.Instance.Current.CurrentTimeline = timeline;
            this.closeAndPlayTimeline(timeline);
        }

        void finishButton_MouseButtonClick(Widget source, EventArgs e)
        {
            String timeline = DataDrivenNavigationManager.Instance.Current.MenuTimeline;
            DataDrivenNavigationManager.Instance.popNavigationState();
            DataDrivenExamController.Instance.popCurrentSection();
            if (DataDrivenNavigationManager.Instance.Count > 0)
            {
                DataDrivenNavigationState state = DataDrivenNavigationManager.Instance.Current;
                state.configureGUI(this);
                state.CurrentTimeline = timeline;
            }
            else
            {
                clearNavigationBar();
                hideNavigationBar();
            }
            this.closeAndPlayTimeline(timeline);
        }
    }
}

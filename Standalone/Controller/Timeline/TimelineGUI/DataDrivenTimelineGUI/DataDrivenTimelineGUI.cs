using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using Engine;
using MyGUIPlugin;
using Logging;
using System.IO;
using Medical.LayoutDataControls;

namespace Medical
{
    public class DataDrivenTimelineGUI : GenericTimelineGUI<DataDrivenTimelineGUIData>
    {
        private const int BUTTON_TO_PANEL_PAD = 20;
        public const int BUTTON_HEIGHT = 28;
        private const int WIDTH_ADJUSTMENT = 10;

        private DataControl topLevelDataControl;
        private DataDrivenExamSection guiSection;

        private Button submitButton;
        private Button cancelButton;
        private Button previousButton;
        private Button nextButton;
        private Button finishButton;

        public DataDrivenTimelineGUI()
            :base("Medical.Controller.Timeline.TimelineGUI.DataDrivenTimelineGUI.DataDrivenTimelineGUI.layout")
        {
            
        }

        public override void Dispose()
        {
            if (submitButton != null)
            {
                Gui.Instance.destroyWidget(submitButton);
            }
            if (cancelButton != null)
            {
                Gui.Instance.destroyWidget(cancelButton);
            }
            if (previousButton != null)
            {
                Gui.Instance.destroyWidget(previousButton);
                Gui.Instance.destroyWidget(nextButton);
                Gui.Instance.destroyWidget(finishButton);
            }
            topLevelDataControl.Dispose();
            base.Dispose();
        }

        protected override void onShown()
        {
            Vector2 startPos = new Vector2(0, 0);
            if (DataDrivenNavigationManager.Instance.Count == 0)
            {
                //Make a cancel and submit button and exam if this was launched directly.
                if (GUIData.AllowSubmit)
                {
                    submitButton = (Button)widget.createWidgetT("Button", "Button", widget.Width - 100 - WIDTH_ADJUSTMENT, 0, 100, BUTTON_HEIGHT, MyGUIPlugin.Align.Default, "");
                    submitButton.MouseButtonClick += new MyGUIEvent(submitButton_MouseButtonClick);
                    submitButton.Caption = GUIData.SubmitButtonText;
                }

                cancelButton = (Button)widget.createWidgetT("Button", "Button", WIDTH_ADJUSTMENT, 0, 100, BUTTON_HEIGHT, MyGUIPlugin.Align.Default, "");
                cancelButton.MouseButtonClick += new MyGUIEvent(cancelButton_MouseButtonClick);
                cancelButton.Caption = GUIData.CancelButtonText;
                startPos.y = cancelButton.Bottom + BUTTON_TO_PANEL_PAD;

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
                int buttonSize = (widget.Width - WIDTH_ADJUSTMENT) / 3;

                previousButton = (Button)widget.createWidgetT("Button", "Button", xLoc, 0, buttonSize, BUTTON_HEIGHT, MyGUIPlugin.Align.Default, "");
                previousButton.MouseButtonClick += new MyGUIEvent(previousButton_MouseButtonClick);
                previousButton.Caption = "Previous";
                xLoc += previousButton.Width;
                previousButton.Enabled = DataDrivenNavigationManager.Instance.Current.PreviousTimeline != null;
                
                nextButton = (Button)widget.createWidgetT("Button", "Button", xLoc, 0, buttonSize, BUTTON_HEIGHT, MyGUIPlugin.Align.Default, "");
                nextButton.MouseButtonClick += new MyGUIEvent(nextButton_MouseButtonClick);
                nextButton.Caption = "Next";
                xLoc += nextButton.Width;
                nextButton.Enabled = DataDrivenNavigationManager.Instance.Current.NextTimeline != null;
                
                finishButton = (Button)widget.createWidgetT("Button", "Button", xLoc, 0, buttonSize, BUTTON_HEIGHT, MyGUIPlugin.Align.Default, "");
                finishButton.MouseButtonClick += new MyGUIEvent(finishButton_MouseButtonClick);
                finishButton.Caption = "Finish";
                xLoc += finishButton.Width;
                finishButton.Enabled = DataDrivenNavigationManager.Instance.Current.MenuTimeline != null;

                startPos.y = previousButton.Bottom + BUTTON_TO_PANEL_PAD;
            }

            if (String.IsNullOrEmpty(GUIData.LayoutFile))
            {
                MyGUIDataControlFactory myGuiFactory = new MyGUIDataControlFactory(widget, this);
                GUIData.createControls(myGuiFactory);
                topLevelDataControl = myGuiFactory.TopLevelControl;
            }
            else
            {
                using(Stream layoutStream = this.openFile(GUIData.LayoutFile))
                {
                    LayoutDataControl layoutDataControl = new LayoutDataControl(layoutStream, widget);
                    LayoutDataControlFactory dataControlFactory = new LayoutDataControlFactory(layoutDataControl, this);
                    GUIData.createControls(dataControlFactory);
                    topLevelDataControl = layoutDataControl;
                }
            }

            topLevelDataControl.displayData(guiSection);
            topLevelDataControl.WorkingSize = new Size2(widget.Width - WIDTH_ADJUSTMENT, 10000);
            topLevelDataControl.Location = startPos;
            topLevelDataControl.layout();
            Size2 desiredSize = new Size2(widget.Width - WIDTH_ADJUSTMENT, topLevelDataControl.Height);
            desiredSize.Height += startPos.y;
            ((ScrollView)widget).CanvasSize = desiredSize;
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
                    if (!String.IsNullOrEmpty(GUIData.PrettyName))
                    {
                        return GUIData.PrettyName;
                    }
                    else
                    {
                        return TimelineFile != null ? TimelineFile : "Not Saved Yet";
                    }
                }
            }
        }

        protected override void closing()
        {
            topLevelDataControl.captureData(guiSection);
        }

        void submitButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (GUIData.PlayTimelineOnSubmit)
            {
                if (!String.IsNullOrEmpty(GUIData.SubmitTimeline))
                {
                    this.closeAndPlayTimeline(GUIData.SubmitTimeline);
                }
                else
                {
                    Log.Warning("Could not play timeline for submit button in Timeline '{0}' because it was not defined.", this.TimelineFile);
                }
            }
            else
            {
                this.closeAndReturnToMainGUI();
            }
            DataDrivenExamController.Instance.saveAndClear();
        }

        void cancelButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (GUIData.PlayTimelineOnCancel)
            {
                if (!String.IsNullOrEmpty(GUIData.CancelTimeline))
                {
                    this.closeAndPlayTimeline(GUIData.CancelTimeline);
                }
                else
                {
                    Log.Warning("Could not play timeline for cancel button in Timeline '{0}' because it was not defined.", this.TimelineFile);
                }
            }
            else
            {
                this.closeAndReturnToMainGUI();
            }
            DataDrivenExamController.Instance.clear();
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

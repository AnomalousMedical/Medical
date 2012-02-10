using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logging;
using MyGUIPlugin;
using Medical.Controller;
using Engine;

namespace Medical.GUI
{
    /// <summary>
    /// A single panel in a TimelineWizard.
    /// 
    /// It must be shown with the TimelineWizard show function or it will not be
    /// disposed properly. That class handles the memory management and has more
    /// info in its description.
    /// </summary>
    public class TimelineWizardPanel : AbstractTimelineGUI
    {
        protected TimelineWizard timelineWizard;
        private TimelineWizardPanelData panelData;

        private Layout layout;
        private Widget subLayoutWidget;
        private ScrollView panelScroll;

        public TimelineWizardPanel(String layoutFile, TimelineWizard timelineWizard)
            : base("Medical.Controller.Timeline.TimelineGUI.TimelineWizard.WizardPanelButtons.layout")
        {
            this.timelineWizard = timelineWizard;

            panelScroll = (ScrollView)widget.findWidget("PanelScroll");

            layout = LayoutManager.Instance.loadLayout(layoutFile);
            subLayoutWidget = layout.getWidget(0);
            subLayoutWidget.attachToWidget(panelScroll);
            panelScroll.CanvasSize = new Size2(subLayoutWidget.Width, subLayoutWidget.Height);

            int buttonAreaHeight = widget.Height;
            subLayoutWidget.setPosition(0, 0);
            subLayoutWidget.Align = Align.Stretch;

            Button cancelButton = (Button)widget.findWidget("StateWizardButtons/Cancel");
            cancelButton.MouseButtonClick += new MyGUIEvent(cancelButton_MouseButtonClick);

            Button finishButton = (Button)widget.findWidget("StateWizardButtons/Finish");
            finishButton.MouseButtonClick += new MyGUIEvent(finishButton_MouseButtonClick);
        }

        public override void Dispose()
        {
            if (layout != null)
            {
                LayoutManager.Instance.unloadLayout(layout);
            }
            base.Dispose();
        }

        public override void initialize(ShowTimelineGUIAction showGUIAction)
        {
            base.initialize(showGUIAction);

            panelData = (TimelineWizardPanelData)showGUIAction.GUIData;

            if (!panelData.AttachToScrollView)
            {
                //Detach the panel and set it up correctly to be on the widget
                subLayoutWidget.detachFromWidget();
                subLayoutWidget.attachToWidget(widget);
                subLayoutWidget.setPosition(0, panelScroll.Top);
                subLayoutWidget.setSize(panelScroll.Width, panelScroll.Height);
                panelScroll.Visible = false;
                subLayoutWidget.Align = Align.Stretch;
            }

            if (!timelineWizard.WizardInterfaceShown && panelData.HasTimelineLinks)
            {
                timelineWizard.clearTimelines();
                clearNavigationBar();
                foreach (TimelineEntry timelineEntry in panelData.Timelines)
                {
                    addToNavigationBar(timelineEntry.Timeline, timelineEntry.Name, timelineEntry.ImageKey);
                    timelineWizard.addTimeline(timelineEntry);
                }
                showNavigationBar();
            }

            timelineWizard.show(this);
            timelineWizard.CurrentTimeline = showGUIAction.Timeline.SourceFile;

            Button nextButton = (Button)widget.findWidget("StateWizardButtons/Next");
            nextButton.MouseButtonClick += new MyGUIEvent(nextButton_MouseButtonClick);
            nextButton.Enabled = !String.IsNullOrEmpty(timelineWizard.NextTimeline);

            Button previousButton = (Button)widget.findWidget("StateWizardButtons/Previous");
            previousButton.MouseButtonClick += new MyGUIEvent(previousButton_MouseButtonClick);
            previousButton.Enabled = !String.IsNullOrEmpty(timelineWizard.PreviousTimeline);
        }

        public virtual void opening(MedicalController medicalController, SimulationScene simScene)
        {

        }

        /// <summary>
        /// This method will be called when one of the navigation buttons
        /// (cancel, next, previous, finish) is pressed or if the user navigates
        /// away with the navigation bar.
        /// </summary>
        protected virtual void commitOutstandingData()
        {

        }

        public IEnumerable<TimelineEntry> Timelines
        {
            get
            {
                return panelData.Timelines;
            }
        }

        public TimelineWizardPanelData PanelData
        {
            get
            {
                return panelData;
            }
        }

        protected override void navigationBarChangedTimelines(string timeline)
        {
            commitOutstandingData();
            base.navigationBarChangedTimelines(timeline);
        }

        void finishButton_MouseButtonClick(Widget source, EventArgs e)
        {
            commitOutstandingData();
            timelineWizard.finish();
            hideNavigationBar();
            closeAndReturnToMainGUI();
        }

        void cancelButton_MouseButtonClick(Widget source, EventArgs e)
        {
            commitOutstandingData();
            timelineWizard.cancel();
            hideNavigationBar();
            closeAndReturnToMainGUI();
        }

        void previousButton_MouseButtonClick(Widget source, EventArgs e)
        {
            commitOutstandingData();
            closeAndPlayTimeline(timelineWizard.PreviousTimeline);
        }

        void nextButton_MouseButtonClick(Widget source, EventArgs e)
        {
            commitOutstandingData();
            closeAndPlayTimeline(timelineWizard.NextTimeline);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logging;
using MyGUIPlugin;
using Medical.Controller;

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

        public TimelineWizardPanel(String layoutFile, TimelineWizard timelineWizard)
            : base("Medical.TimelineGUI.WizardPanelButtons.layout")
        {
            this.timelineWizard = timelineWizard;

            layout = LayoutManager.Instance.loadLayout(layoutFile);
            Widget subLayoutWidget = layout.getWidget(0);
            subLayoutWidget.attachToWidget(widget);

            int buttonAreaHeight = widget.Height;
            subLayoutWidget.setPosition(0, buttonAreaHeight);
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

        public LayoutContainer Container
        {
            get
            {
                return layoutContainer;
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

        protected void applyLayers(LayerState layers)
        {
            if (layers != null)
            {
                layers.apply();
            }
        }

        protected void applyCameraPosition(CameraPosition cameraPosition)
        {
            SceneViewWindow window = timelineWizard.SceneViewController.ActiveWindow;
            if (window != null)
            {
                window.setPosition(cameraPosition);
            }
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

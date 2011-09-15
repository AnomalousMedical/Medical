using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logging;
using MyGUIPlugin;

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
        }

        public override void Dispose()
        {
            if (layout != null)
            {
                LayoutManager.Instance.unloadLayout(layout);
            }
            timelineWizard.hide();
            base.Dispose();
        }

        public override void initialize(ShowTimelineGUIAction showGUIAction)
        {
            base.initialize(showGUIAction);
            panelData = (TimelineWizardPanelData)showGUIAction.GUIData;
            timelineWizard.show(this);

            if (panelData.HasTimelineLinks)
            {
                clearNavigationBar();
                foreach (TimelineEntry timelineEntry in panelData.Timelines)
                {
                    addToNavigationBar(timelineEntry.Timeline, timelineEntry.Name, timelineEntry.ImageKey);
                }
                showNavigationBar();
            }
        }

        public virtual void opening(MedicalController medicalController, SimulationScene simScene)
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
    }
}

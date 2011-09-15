using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logging;

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

        public TimelineWizardPanel(String layoutFile, TimelineWizard timelineWizard)
            :base(layoutFile)
        {
            this.timelineWizard = timelineWizard;
        }

        public override void Dispose()
        {
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
